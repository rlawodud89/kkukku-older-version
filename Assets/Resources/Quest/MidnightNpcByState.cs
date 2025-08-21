// MidnightNpcByState.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class MidnightNpcByState : MonoBehaviour, IPointerClickHandler
{
    [Header("공유 상태 SO (필수)")]
    public QuestChainStateSO state;

    [Header("이 NPC가 등장/동작할 체인 단계 인덱스")]
    public int stepIndex = 1; // 담요 전달 단계

    [Header("이 단계의 QuestSO (isCompleted/getReward만 사용)")]
    public QuestSO stepQuest; // 1단계 SO를 넣어두면 됨

    [Header("턴인 요구 아이템")]
    public string requiredItemId = "blanket_lilac_dream";

    [Header("보상 자동 수령(=getReward true)")]
    public bool autoClaimReward = true;

    [Header("인벤토리 연결(IInventory 구현체)")]
    public MonoBehaviour inventoryProvider; // IInventory
    private IInventory Inv => inventoryProvider as IInventory;

    [Header("활성/비활성 토글 대상 (비우면 자기 자신)")]
    public GameObject rootToToggle;

    private void Awake()
    {
        if (rootToToggle == null) rootToToggle = gameObject;
        // 씬 저장 시 비활성 권장. 혹시 켜져 있으면 상태 체크로 통일
        rootToToggle.SetActive(false);
    }

    private void OnEnable()
    {
        if (state != null) state.OnIndexChanged += HandleIndexChanged;
        // 초기 반영
        HandleIndexChanged(state != null ? state.CurrentIndex : -1);
    }

    private void OnDisable()
    {
        if (state != null) state.OnIndexChanged -= HandleIndexChanged;
    }

    private void HandleIndexChanged(int newIndex)
    {
        bool shouldShow = (newIndex == stepIndex) && !(stepQuest != null && stepQuest.isCompleted);
        // (선택) 더 엄격히: 전달 단계가 맞는지까지 체크하려면 extra flag/ReqType을 여기로 가져오면 됨
        if (rootToToggle.activeSelf != shouldShow) rootToToggle.SetActive(shouldShow);
    }

    public void OnPointerClick(PointerEventData eventData) => TryTurnIn();
    private void OnMouseDown() => TryTurnIn(); // 3D 클릭용

    private void TryTurnIn()
    {
        if (state == null || stepQuest == null || Inv == null) return;
        if (state.CurrentIndex != stepIndex) return;      // 내 차례가 아니면 무시
        if (stepQuest.isCompleted) return;                // 이미 완료면 무시

        // 인벤토리 체크
        if (Inv.GetCount(requiredItemId) < 1)
        {
            // TODO: UI 피드백 “라일락꿈 담요가 필요해요”
            return;
        }

        // 아이템 소모 & 완료 플래그
        Inv.Remove(requiredItemId, 1);
        stepQuest.isCompleted = true;

        // 보상 자동 수령 → 러너가 감시하다가 다음 단계로 이동
        if (autoClaimReward) stepQuest.getReward = true;

        // 바로 숨기기(선택)
        if (rootToToggle.activeSelf) rootToToggle.SetActive(false);
    }

    // 프로젝트 인벤토리 인터페이스
    public interface IInventory
    {
        int GetCount(string itemId);
        void Add(string itemId, int amount);
        bool Remove(string itemId, int amount);
    }
}
