// MidnightNpcByState.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidnightNpcByState : MonoBehaviour
{
    private GameManager gameManager;

    [Header("공유 상태 SO (필수)")]
    public QuestChainStateSO state;

    [Serializable]
    public class Scenario
    {
        public int stepIndex;                 // 예: 1, 5
        public QuestSO stepQuest;

        [Header("턴인 요구 아이템 (비우면 '대화로 완료')")]
        public string requiredItemName = "";
        public int requiredCount = 1;

        [Header("턴인 직후 자동으로 getReward 처리할지(보상 패널 없이 즉시 다음 단계)")]
        public bool autoClaimReward = false;  // ★ 보상 패널 쓸 거면 false
    }

    [Header("이 NPC의 모든 시나리오들(1단계, 5단계 등)")]
    public List<Scenario> scenarios = new();

    [Header("활성/비활성 토글 대상 (비우면 자기 자신)")]
    public GameObject rootToToggle;

    // ===== 연출(감사 대사) =====
    [Header("연출: 감사 대사 패널(선택)")]
    public GameObject thanksPanel;                       // 초기 비활성
    public TMPro.TextMeshProUGUI thanksLabel;            // 선택
    [TextArea] public string thanksText = "고마워요. 덕분에 고요한 꿈을 꿀 수 있을 것 같아요.";
    public float thanksSeconds = 1.0f;
    public bool hideNpcAfterThanks = true;
    private bool _turnInLock;

    private IEnumerator Start()
    {
        if (rootToToggle == null) rootToToggle = gameObject;

        while (gameManager == null)
        {
            gameManager = GameManager.getInstance();
            yield return null;
        }

        if (state != null) state.OnIndexChanged += HandleIndexChanged;
        HandleIndexChanged(state != null ? state.CurrentIndex : -1);
    }

    private void OnDestroy()
    {
        if (state != null) state.OnIndexChanged -= HandleIndexChanged;
    }

    private void HandleIndexChanged(int newIndex)
    {
        var sc = GetActiveScenario(newIndex);
        bool completed = (sc?.stepQuest != null) && sc.stepQuest.isCompleted;
        bool shouldShow = (sc != null) && sc.stepQuest != null && !completed;

        Debug.Log($"[Midnight] idx={newIndex}, sc={(sc != null ? sc.stepIndex.ToString() : "null")}, " +
                  $"quest={(sc?.stepQuest ? sc.stepQuest.name : "null")}, " +
                  $"isCompleted={completed}, rootActive={(rootToToggle ? rootToToggle.activeSelf : (bool?)null)}, " +
                  $"stateID={(state ? state.GetInstanceID() : 0)}");

        if (rootToToggle && rootToToggle.activeSelf != shouldShow)
            rootToToggle.SetActive(shouldShow);
    }

    public void OnClickButton() { TryInteract(); } // UI Button.onClick에 연결

    private void TryInteract()
    {
        if (_turnInLock) return;
        _turnInLock = true;

        if (state == null) { _turnInLock = false; return; }

        var sc = GetActiveScenario(state.CurrentIndex);
        if (sc == null || sc.stepQuest == null) { _turnInLock = false; return; }
        if (sc.stepQuest.isCompleted) { _turnInLock = false; return; }

        // (A) 대화 완료형
        if (string.IsNullOrWhiteSpace(sc.requiredItemName))
        {
            OnTurnInSucceeded(sc);
            return;
        }

        // (B) 아이템 턴인
        int have = (gameManager != null) ? gameManager.Count_InventoryItem(sc.requiredItemName.Trim()) : 0;
        if (have < sc.requiredCount)
        {
            // TODO: 토스트/말풍선 "○○이 필요해요"
            _turnInLock = false; return;
        }

        gameManager.Use_InventoryItem(sc.requiredItemName.Trim(), sc.requiredCount);
        OnTurnInSucceeded(sc);
    }

    private void OnTurnInSucceeded(Scenario sc)
    {
        // 1) 단계 완료 표시
        sc.stepQuest.isCompleted = true;

        // 2) 감사 대사 잠깐
        if (thanksPanel != null)
        {
            if (thanksLabel != null) thanksLabel.text = thanksText;
            thanksPanel.SetActive(true);
            Invoke(nameof(HideThanksPanel), Mathf.Max(0.05f, thanksSeconds));
        }

        // 3) 미드나잇 숨김
        if (hideNpcAfterThanks && rootToToggle.activeSelf)
            rootToToggle.SetActive(false);

        // ★ 보상 패널을 QuestManager에서 열게 함
        float delay = (thanksPanel != null) ? thanksSeconds : 0f;
        StartCoroutine(OpenRewardAfterDelay(sc.stepQuest, delay));
    }

    private void HideThanksPanel()
    {
        if (thanksPanel != null) thanksPanel.SetActive(false);
    }

    private IEnumerator OpenRewardAfterDelay(QuestSO quest, float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        if (thanksPanel != null) thanksPanel.SetActive(false);

        var qm = QuestManager.Instance;
        if (qm != null) qm.SpecialQuestGetReward(quest);
        else Debug.LogWarning("[MidnightNPC] QuestManager.Instance is null → 보상 패널을 열 수 없습니다.");

        _turnInLock = false;
    }

    // 인덱스 매칭 실패 시, 현재 단계의 퀘스트와 '참조 동일성'으로도 매칭 시도
    private Scenario GetActiveScenario(int idx)
    {
        // 1) 인덱스 매칭 우선
        for (int i = 0; i < scenarios.Count; i++)
            if (scenarios[i].stepIndex == idx) return scenarios[i];

        // 2) 실패하면 같은 state를 쓰는 러너가 있으면 현재 단계 QuestSO로 참조 매칭
        if (QuestChainRunner.TryGetRunner(state, out var runner))
        {
            var curQuest = runner.GetQuestAt(idx);
            if (curQuest)
            {
                for (int i = 0; i < scenarios.Count; i++)
                    if (scenarios[i].stepQuest == curQuest) return scenarios[i];
            }
        }
        return null;
    }
}
