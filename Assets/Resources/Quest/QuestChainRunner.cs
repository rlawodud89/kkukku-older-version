// QuestChainRunner.cs
// 상태 저장소 ScriptableObject(QuestChainStateSO)를 공유하여
// 다른 씬의 NPC가 러너 참조 없이도 현재 단계를 알 수 있도록 구성.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
public class QuestChainRunner : MonoBehaviour
{
    // 호출 위치: OnEnable()나 Start() 초반
    private void PrepareRuntimeQuests()
    {
        if (_runtimePrepared) return;
        _runtimePrepared = true;

        _baseDescriptions.Clear();
        for (int i = 0; i < steps.Count; i++)
        {
            var q = steps[i].quest;
            if (q == null) { _baseDescriptions.Add(""); continue; }

            // 원본 본문 백업
            _baseDescriptions.Add(q.questDescription ?? "");

            // 런타임 클론 생성 → steps[i].quest 교체
            var clone = ScriptableObject.Instantiate(q);
            clone.name = q.name + "_Runtime";
#if UNITY_EDITOR
            clone.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
#else
        clone.hideFlags = HideFlags.DontSave;
#endif
            steps[i].quest = clone;
        }
    }


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

    [Header("외부 시스템 연결")]
    [SerializeField] private MonoBehaviour inventoryProvider; // IInventory 구현체

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

    private void Start()
    {
        gameManager = GameManager.getInstance();
    }
    private void OnEnable()
    {
        PrepareRuntimeQuests();

        if (state == null)
            Debug.LogWarning("QuestChainStateSO가 연결되지 않았습니다. 씬 간 동기화를 원하면 반드시 할당하세요.");

        // 러너가 '소스 오브 트루스'로서 현재 인덱스를 상태 SO에 반영
        if (state != null)
        {
            state.SetIndex(currentIndex);
            state.SetChainCompleted(IsChainFinished());
            state.OnIndexChanged += HandleExternalIndexChange; // 선택: 외부에서 인덱스를 바꾸는 흐름 허용
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


        //if (Inv.GetCount(itemId) < amount) return false;
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

                /*
                // 메인 요구
                if (Inv.GetCount(s.requirement.itemId) < Mathf.Max(1, s.requirement.requiredCount))
                    return false;

                // 추가 수집
                foreach (var e in s.requirement.extraCollects)
                    if (Inv.GetCount(e.itemId) < e.count)
                        return false;*/

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

        var idx = currentIndex;
        var s = steps[idx];
        var q = s.quest;

        // 1) 원본(에셋)에서 백업해 둔 베이스 본문
        string baseText = (idx < _baseDescriptions.Count) ? _baseDescriptions[idx] : (q != null ? (q.questDescription ?? "") : "");

        // 2) 진행도 블록 계산(CollectItems일 때만)
        string progress = BuildProgressBlock(s);

        // 3) 주입(토큰이 있으면 그 구간만, 없으면 아래에 붙임)
        string composed = InjectProgress(baseText, progress);

        // 4) 런타임 클론 SO의 description만 갱신(에셋 파일은 건드리지 않음)
        if (q != null && q.questDescription != composed)
            q.questDescription = composed;

        // 5) 기존 훅도 그대로 호출(원하면 이걸로 UI를 직접 갱신)
        onJournalUpdate?.Invoke(idx, composed);
    }


    private const string PROG_BEGIN = "<PROGRESS>";
    private const string PROG_END = "</PROGRESS>";

    private string BuildProgressBlock(Step s)
    {
        // CollectItems일 때만 진행도 표기
        if (s.requirement.type != ReqType.CollectItems) return null;

        var lines = new System.Text.StringBuilder();
        // 메인 목표
        //lines.AppendLine($"{s.requirement.itemId} ({GetCount(s.requirement.itemId)}/{Mathf.Max(1, s.requirement.requiredCount)})");

        // 추가 목표
        var extras = s.requirement.extraCollects;
        if (extras != null)
        {
            for (int i = 0; i < extras.Length; i++)
            {
                var e = extras[i];
                //lines.AppendLine($"{e.itemId} ({GetCount(e.itemId)}/{e.count})");
            }
        }

        return lines.ToString().TrimEnd();
    }

    private string InjectProgress(string baseText, string progressBlock)
    {
        if (string.IsNullOrEmpty(progressBlock)) return baseText ?? "";

        if (string.IsNullOrEmpty(baseText))
            return progressBlock;

        int i0 = baseText.IndexOf(PROG_BEGIN, StringComparison.Ordinal);
        int i1 = baseText.IndexOf(PROG_END, StringComparison.Ordinal);

        if (i0 != -1 && i1 != -1 && i1 > i0)
        {
            // 토큰 사이만 교체
            var before = baseText.Substring(0, i0 + PROG_BEGIN.Length);
            var after = baseText.Substring(i1);
            return $"{before}\n{progressBlock}\n{after}";
        }
        // 토큰이 없으면 맨 아래에 덧붙임
        return $"{baseText.TrimEnd()}\n{progressBlock}";
    }





    // ====== 외부에서 참조하기 쉬운 헬퍼 (NPC 등에서 사용) ======
    public int CurrentIndex => currentIndex;
    public bool IsCurrentStep(int index) => !IsChainFinished() && currentIndex == index;
    public QuestSO GetQuestAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].quest : null;
    public ReqType GetReqTypeAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.type : ReqType.None;
    public string GetReqItemIdAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.itemId : null;
}

