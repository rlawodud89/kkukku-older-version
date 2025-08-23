// QuestChainRunner.cs
// 상태 저장소 ScriptableObject(QuestChainStateSO)를 공유하여
// 다른 씬의 NPC가 러너 참조 없이도 현재 단계를 알 수 있도록 구성.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using System.Collections;

public class QuestChainRunner : MonoBehaviour
{
    private GameManager gameManager;
    private bool _runtimePrepared = false;
    private List<string> _baseDescriptions = new(); // 원본 본문 백업(진행도 덧붙일 때 기준)

 


    // ====== 단계 정의 ======
    [Serializable]
    public class Step
    {
        [Tooltip("에디터 식별용 이름(예: 0_담요요청, 1_담요전달, 2_맞춤이불안내, 3_재료수집, 4_제작)")]
        public string stepName;

        [Tooltip("이 단계의 퀘스트 SO(questDescription이 저널 텍스트의 본문으로 사용됩니다)")]
        public QuestSO quest;

        [Tooltip("이 단계 완료 조건")]
        public StepRequirement requirement;
    }

    public enum ReqType
    {
        None,           // 별도 조건 없이 연출용(보상 버튼 눌러 다음 단계로 이동)
        CollectItems,   // 인벤토리 수량이 모두 조건 이상이면 완료
        TurnInItem,     // 인벤토리에 아이템이 있고 TryTurnInCurrent() 호출 시 완료
        CraftRecipeFlag // 제작 성공 외부 알림(SetCraftedFlag) 수신 시 완료
    }

    [Serializable]
    public class StepRequirement
    {
        public ReqType type = ReqType.None;

        [Header("CollectItems / TurnInItem / Craft")]
        [Tooltip("메인 아이템 ID(수집/턴인/제작식별)")]
        public string itemId;

        [Tooltip("CollectItems에서만 사용(메인 요구 수량)")]
        public int requiredCount = 1;

        [Header("CollectItems 추가 수집 항목들")]
        public CollectEntry[] extraCollects;

        [Serializable]
        public struct CollectEntry { public string itemId; public int count; }
    }

    // ====== 인스펙터 ======
    [Header("체인 단계들(순서대로 배치)")]
    public List<Step> steps = new();



    [Header("공유 상태 SO (씬 간 연결의 핵심)")]
    public QuestChainStateSO state; // 같은 에셋을 모든 씬에서 참조

    [Header("이벤트 훅")]
    public UnityEvent<int, string> onJournalUpdate; // (현재단계인덱스, 텍스트)
    public UnityEvent<int> onStepBecameCompleted;
    public UnityEvent onChainCompleted;

    // 진행 인덱스(모든 단계 완료 시 = steps.Count)
    [SerializeField, ReadOnly] private int currentIndex = 0;

    // 제작 성공/특정 산출물 플래그
    private readonly HashSet<string> craftedFlags = new();


    // ====== Unity ======

    private IEnumerator Start()
    {
        // GameManager 인스턴스가 유효할 때까지 대기
        while (gameManager == null)
        {
            gameManager = GameManager.getInstance();
            yield return null; // 한 프레임 대기
        }


        if (state == null)
            Debug.LogWarning("QuestChainStateSO가 연결되지 않았습니다.");

        if (state != null)
        {
            state.SetIndex(currentIndex);
            state.SetChainCompleted(IsChainFinished());
            state.OnIndexChanged += HandleExternalIndexChange;
        }

        RefreshJournal();
    }

    private void OnDisable()
    {
        if (state != null)
            state.OnIndexChanged -= HandleExternalIndexChange;
    }

    // 외부(디버그 도구/에디터/다른 시스템)가 인덱스를 변경한 경우 러너도 맞춤
    private void HandleExternalIndexChange(int newIndex)
    {
        if (newIndex == currentIndex) return;
        currentIndex = Mathf.Clamp(newIndex, 0, steps.Count);
        RefreshJournal();
    }

    private void Update()
    {
        if (IsChainFinished()) return;

        var step = steps[currentIndex];
        var q = step.quest;
        if (q == null) { Advance(); return; }

        // 1) 조건 충족 → isCompleted = true
        if (!q.isCompleted)
        {
            if (CheckRequirementMet(step))
            {
                q.isCompleted = true;
                onStepBecameCompleted?.Invoke(currentIndex);
                RefreshJournal();
            }
            else
            {
                // 수집 단계면 진행도 실시간 갱신
                if (step.requirement.type == ReqType.CollectItems)
                    RefreshJournal();
            }
        }
        // 2) 완료 후 getReward가 true면 다음 단계로
        else
        {
            if (q.getReward)
            {
                Advance();
            }
        }
    }

    // ====== 외부에서 호출하는 API ======

    /// <summary>현재 단계가 TurnInItem일 때: 대화/상호작용에서 호출</summary>
    public bool TryTurnInCurrent(string itemId, int amount = 1)
    {
        if (IsChainFinished()) return false;
        var s = steps[currentIndex];
        if (s.requirement.type != ReqType.TurnInItem) return false;
        if (s.requirement.itemId != itemId) return false;


        if (gameManager.Count_InventoryItem(itemId) < amount) return false;
        gameManager.Use_InventoryItem(itemId, amount);
        

        s.quest.isCompleted = true;
        onStepBecameCompleted?.Invoke(currentIndex);
        RefreshJournal();
        return true;
    }

    /// <summary>제작 성공 시 호출(예: '별무늬고요 이불' 완제 성공 시)</summary>
    public void SetCraftedFlag(string productItemId)
    {
        craftedFlags.Add(productItemId);
    }

    /// <summary>체인을 특정 인덱스부터 시작/재시작(세이브 복원 시 유용)</summary>
    public void BeginChainAt(int idx = 0)
    {
        currentIndex = Mathf.Clamp(idx, 0, steps.Count);
        if (state != null)
        {
            state.SetChainCompleted(IsChainFinished());
            state.SetIndex(currentIndex);
        }
        RefreshJournal();
    }

    // ====== 내부 로직 ======
    private bool CheckRequirementMet(Step s)
    {
        switch (s.requirement.type)
        {
            case ReqType.None:
                return true;

            case ReqType.CollectItems:

                Debug.Log($"[INV]: {gameManager.Count_InventoryItem(s.requirement.itemId)}");
                // 메인 요구
                if (gameManager.Count_InventoryItem(s.requirement.itemId) < Mathf.Max(1, s.requirement.requiredCount))
                    return false;

                // 추가 수집
                foreach (var e in s.requirement.extraCollects)
                {
                    Debug.Log($"[INV]: {gameManager.Count_InventoryItem(e.itemId)}");
                    if (gameManager.Count_InventoryItem(e.itemId) < e.count)
                        return false;
                }

                return true;

            case ReqType.TurnInItem:
                // 실제 완료 처리는 TryTurnInCurrent에서 실행
                return false;

            case ReqType.CraftRecipeFlag:
                return craftedFlags.Contains(s.requirement.itemId);

            default:
                return false;
        }
    }

    private void Advance()
    {
        currentIndex++;
        if (state != null) state.SetIndex(currentIndex);

        if (IsChainFinished())
        {
            onChainCompleted?.Invoke();
            if (state != null) state.SetChainCompleted(true);
            return;
        }

        RefreshJournal();
    }

    private bool IsChainFinished() => currentIndex >= steps.Count;

    private void RefreshJournal()
    {
        if (IsChainFinished()) return;

        var s = steps[currentIndex];
        var q = s.quest;

        // SO의 questDescription을 직접 수정
        if (s.requirement.type == ReqType.CollectItems)
        {
            UpdateQuestDescriptionProgress(s);
        }
        else
        {
            // 수집 단계가 아니면 원문 그대로 이벤트로 흘려보냄
            string text = q != null ? (q.questDescription ?? string.Empty) : string.Empty;
            onJournalUpdate?.Invoke(currentIndex, text);
        }
    }

    // 진행도 라인 텍스트를 "(현재/필요)"로 바꿔 끼우는 헬퍼
    private void UpdateQuestDescriptionProgress(Step s)
    {
        if (!Application.isPlaying) return;           // 에디터 편집 중엔 에셋 수정 안 함
        if (s.quest == null) return;
        if (s.requirement.type != ReqType.CollectItems) return;

        string text = s.quest.questDescription ?? string.Empty;
        int replaced = 0;

        // itemName으로 시작하고 "(숫자/숫자)"가 붙은 라인을 찾아 교체 (멀티라인)
        void ReplaceLine(ref string t, string itemName, int cur, int req)
        {
            var pattern = @"(^|\n)" + Regex.Escape(itemName) + @"\s*\(\s*\d+\s*/\s*\d+\s*\)";
            var replacement = "$1" + itemName + $" ({cur}/{req})";
            string newText = Regex.Replace(t, pattern, replacement, RegexOptions.Multiline);

            if (!ReferenceEquals(newText, t))
            {
                replaced++;
                t = newText;
            }
        }

        // 메인 + 추가 항목들 진행도 계산
        ReplaceLine(ref text, s.requirement.itemId, gameManager.Count_InventoryItem(s.requirement.itemId), Mathf.Max(1, s.requirement.requiredCount));
        var extras = s.requirement.extraCollects;
        if (extras != null)
        {
            for (int i = 0; i < extras.Length; i++)
                ReplaceLine(ref text, extras[i].itemId, gameManager.Count_InventoryItem(extras[i].itemId), extras[i].count);
        }

        // 교체된 라인이 하나도 없으면 본문 아래에 블록으로 추가
        if (replaced == 0)
        {
            var sb = new System.Text.StringBuilder(text.TrimEnd());
            if (sb.Length > 0) sb.Append('\n');

            sb.AppendLine($"{s.requirement.itemId} ({gameManager.Count_InventoryItem(s.requirement.itemId)}/{Mathf.Max(1, s.requirement.requiredCount)})");
            if (extras != null)
                for (int i = 0; i < extras.Length; i++)
                    sb.AppendLine($"{extras[i].itemId} ({gameManager.Count_InventoryItem(extras[i].itemId)}/{extras[i].count})");

            text = sb.ToString().TrimEnd();
        }

        // SO의 description을 직접 갱신 (Play 중엔 저장되지 않고, 플레이 종료 시 원복됨)
        if (s.quest.questDescription != text)
            s.quest.questDescription = text;

        // 기존 이벤트 훅도 갱신 텍스트로 호출
        onJournalUpdate?.Invoke(currentIndex, text);
    }

    // (선택) 특정 토큰 블록만 바꾸고 싶을 때 사용 가능
    private string ReplaceBetween(string src, string beginTag, string endTag, string newContent)
    {
        int i0 = src.IndexOf(beginTag, StringComparison.Ordinal);
        int i1 = src.IndexOf(endTag, StringComparison.Ordinal);
        if (i0 >= 0 && i1 > i0)
            return src.Substring(0, i0 + beginTag.Length) + "\n" + newContent + "\n" + src.Substring(i1);
        return src;
    }



    // ====== 외부에서 참조하기 쉬운 헬퍼 (NPC 등에서 사용) ======
    public int CurrentIndex => currentIndex;
    public bool IsCurrentStep(int index) => !IsChainFinished() && currentIndex == index;
    public QuestSO GetQuestAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].quest : null;
    public ReqType GetReqTypeAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.type : ReqType.None;
    public string GetReqItemIdAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.itemId : null;
}

