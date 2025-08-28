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

    [Header("이벤트 채널(씬 간 브로드캐스트)")]
    public VoidEventChannelSO craftStepAppearedChannel;
    public VoidEventChannelSO craftStepCompletedChannel;


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

    [Header("체인 완료 시 지급할 편지(선택)")]
    [Tooltip("체인(모든 단계) 완료하면 이 이름의 편지를 한번만 지급합니다.")]
    public string letterNameOnComplete;   // 예: "제대로 된 이불"
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

        MaybeNotifyAppear();
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
        MaybeNotifyAppear(); // 외부에서 인덱스 바뀐 경우도 체크
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

                MaybeNotifyComplete(step);

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
                {
                    // 메인
                    string mainIdRaw = s.requirement.itemId;
                    string mainId = mainIdRaw?.Trim();
                    if (string.IsNullOrEmpty(mainId))
                    {
                        Debug.LogError($"[Runner] Step '{s.stepName}' 메인 itemId가 비어있습니다. (원본='{mainIdRaw}')");
                        return false;
                    }

                    int mainHave = gameManager.Count_InventoryItem(mainId);
                    Debug.Log($"[Collect] Main '{mainId}': {mainHave}/{Mathf.Max(1, s.requirement.requiredCount)}");

                    if (mainHave < Mathf.Max(1, s.requirement.requiredCount)) return false;

                    // 추가
                    var extras = s.requirement.extraCollects;
                    if (extras != null && extras.Length > 0)
                    {
                        for (int i = 0; i < extras.Length; i++)
                        {
                            string raw = extras[i].itemId;
                            string id = raw?.Trim();

                            if (string.IsNullOrEmpty(id))
                            {
                                Debug.LogWarning($"[Collect] Step '{s.stepName}' extraCollects[{i}] 가 비어있습니다. (원본='{raw}') → 이 항목은 건너뜀");
                                continue; // 또는 return false; 로 강제 실패 처리해도 됨
                            }

                            int need = Mathf.Max(1, extras[i].count);
                            int have = gameManager.Count_InventoryItem(id);
                            Debug.Log($"[Collect] Extra '{id}': {have}/{need}");

                            if (have < need) return false;
                        }
                    }

                    return true;
                }

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
            // ★ 여기서 편지 지급 시도
            TryGrantCompletionLetterOnce();

            onChainCompleted?.Invoke();
            if (state != null) state.SetChainCompleted(true);
            return;
        }

        RefreshJournal();
        MaybeNotifyAppear();
    }

    private void MaybeNotifyAppear()
    {
        if (IsChainFinished()) return;
        var s = steps[currentIndex];
        if (s.requirement.type == ReqType.CraftRecipeFlag)
        {
            Debug.Log($"[Runner] Craft step APPEARED at idx={currentIndex}, channel={craftStepAppearedChannel?.name}");
            craftStepAppearedChannel?.Raise();
        }
    }
    private void MaybeNotifyComplete(Step s)
    {
        if (s.requirement.type == ReqType.CraftRecipeFlag && s.quest.isCompleted)
        {
            Debug.Log($"[Runner] Craft step COMPLETED, channel={craftStepCompletedChannel?.name}");
            craftStepCompletedChannel?.Raise();
        }
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
        if (!Application.isPlaying || s.quest == null || s.requirement.type != ReqType.CollectItems) return;

        string text = s.quest.questDescription ?? string.Empty;
        int replaced = 0;

        void ReplaceLineSafe(ref string t, string rawId, int cur, int req)
        {
            var id = rawId?.Trim();
            if (string.IsNullOrEmpty(id)) return;
            var pattern = @"(^|\n)" + Regex.Escape(id) + @"\s*\(\s*\d+\s*/\s*\d+\s*\)";
            var replacement = "$1" + id + $" ({cur}/{req})";
            var newText = Regex.Replace(t, pattern, replacement, RegexOptions.Multiline);
            if (!ReferenceEquals(newText, t)) { replaced++; t = newText; }
        }

        // 메인
        var mainId = s.requirement.itemId?.Trim();
        int mainNeed = Mathf.Max(1, s.requirement.requiredCount);
        int mainCur = string.IsNullOrEmpty(mainId) ? 0 : gameManager.Count_InventoryItem(mainId);
        ReplaceLineSafe(ref text, mainId, mainCur, mainNeed);

        // 추가
        var extras = s.requirement.extraCollects;
        if (extras != null)
        {
            for (int i = 0; i < extras.Length; i++)
            {
                var id = extras[i].itemId?.Trim();
                if (string.IsNullOrEmpty(id)) continue; // ← 빈 슬롯 무시
                int cur = gameManager.Count_InventoryItem(id);
                int need = Mathf.Max(1, extras[i].count);
                ReplaceLineSafe(ref text, id, cur, need);
            }
        }

        if (replaced == 0)
        {
            var sb = new System.Text.StringBuilder(text.TrimEnd());
            if (sb.Length > 0) sb.Append('\n');
            if (!string.IsNullOrEmpty(mainId))
                sb.AppendLine($"{mainId} ({mainCur}/{mainNeed})");

            if (extras != null)
            {
                for (int i = 0; i < extras.Length; i++)
                {
                    var id = extras[i].itemId?.Trim();
                    if (string.IsNullOrEmpty(id)) continue;
                    int cur = gameManager.Count_InventoryItem(id);
                    sb.AppendLine($"{id} ({cur}/{extras[i].count})");
                }
            }
            text = sb.ToString().TrimEnd();
        }

        if (s.quest.questDescription != text)
            s.quest.questDescription = text;

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

    
    // ===[ 완료 편지 지급 헬퍼 ]==========================
    private void TryGrantCompletionLetterOnce()
    {
        // 이름이 비었으면 아무 것도 안 함
        var name = letterNameOnComplete?.Trim();
        if (string.IsNullOrEmpty(name) || gameManager == null) return;

        // 이미 있는지 확인(중복 방지)
        var currentLetters = gameManager.Get_Current_Letter(); // List<LetterScript>
        bool alreadyHas = currentLetters != null && currentLetters.Exists(l => l.title == name);
        if (alreadyHas) return;

        // 지급
        Debug.Log($"[QuestChainRunner] 체인 완료! 편지 지급: {name}");
        gameManager.Add_Letter(name);
    }

    // ====== 외부에서 참조하기 쉬운 헬퍼 (NPC 등에서 사용) ======
    public int CurrentIndex => currentIndex;
    public bool IsCurrentStep(int index) => !IsChainFinished() && currentIndex == index;
    public QuestSO GetQuestAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].quest : null;
    public ReqType GetReqTypeAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.type : ReqType.None;
    public string GetReqItemIdAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.itemId : null;


}

