// QuestChainRunner.cs
// 상태 저장소 ScriptableObject(QuestChainStateSO)를 공유하여
// 다른 씬의 NPC가 러너 참조 없이도 현재 단계를 알 수 있도록 구성.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class QuestChainRunner : MonoBehaviour
{
    // ====== 외부 인벤토리 인터페이스(프로젝트 쪽 구현을 여기에 연결) ======
    public interface IInventory
    {
        int GetCount(string itemId);
        void Add(string itemId, int amount);
        bool Remove(string itemId, int amount);
    }

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
    private IInventory Inv => inventoryProvider as IInventory;

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

    // 보기 좋은 이름 매핑(인게임 표기명 ← itemId)
    private readonly Dictionary<string, string> _readable = new Dictionary<string, string>()
    {
        // 담요
        { "blanket_lilac_dream", "라일락꿈 담요" },
        { "blanket_star_quiet", "별무늬고요 이불" },

        // 새 재료들
        { "thread_galaxy_dream", "은하꿈실" },
        { "cotton_sunlight_mist", "햇빛운무솜" },
        { "petal_dreamlike", "몽환의꽃잎" },
        { "fragment_moon_bluefield", "청야달조각" },
    };

    // ====== Unity ======
    private void OnEnable()
    {
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
        if (!InvSafe()) return false;

        if (Inv.GetCount(itemId) < amount) return false;
        Inv.Remove(itemId, amount);

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
                if (!InvSafe()) return false;

                // 메인 요구
                if (Inv.GetCount(s.requirement.itemId) < Mathf.Max(1, s.requirement.requiredCount))
                    return false;

                // 추가 수집
                foreach (var e in s.requirement.extraCollects)
                    if (Inv.GetCount(e.itemId) < e.count)
                        return false;

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

        // 본문: QuestSO.questDescription 그대로 사용
        string text = q != null ? (q.questDescription ?? string.Empty) : string.Empty;

        // 수집 단계면 진행도 줄 추가
        if (s.requirement.type == ReqType.CollectItems && InvSafe())
        {
            var lines = new List<string>();

            // 메인 목표
            lines.Add($"{Readable(s.requirement.itemId)} ({Inv.GetCount(s.requirement.itemId)}/{Mathf.Max(1, s.requirement.requiredCount)})");

            // 추가 목표
            foreach (var e in s.requirement.extraCollects)
                lines.Add($"{Readable(e.itemId)} ({Inv.GetCount(e.itemId)}/{e.count})");

            if (!string.IsNullOrWhiteSpace(text))
                text += "\n";

            text += string.Join("\n", lines);
        }

        onJournalUpdate?.Invoke(currentIndex, text);
    }

    private string Readable(string itemId)
        => _readable.TryGetValue(itemId, out var pretty) ? pretty : itemId;

    private bool InvSafe()
    {
        if (Inv == null)
        {
            Debug.LogWarning("IInventory 구현체가 연결되지 않았습니다.");
            return false;
        }
        return true;
    }

    // ====== 외부에서 참조하기 쉬운 헬퍼 (NPC 등에서 사용) ======
    public int CurrentIndex => currentIndex;
    public bool IsCurrentStep(int index) => !IsChainFinished() && currentIndex == index;
    public QuestSO GetQuestAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].quest : null;
    public ReqType GetReqTypeAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.type : ReqType.None;
    public string GetReqItemIdAt(int index) => (index >= 0 && index < steps.Count) ? steps[index].requirement.itemId : null;
}

