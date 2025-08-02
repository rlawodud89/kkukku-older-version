using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopController1 : MonoBehaviour
{
    [Header("업그레이드 버튼들")]
    [SerializeField] Button loomButton;        // 직조기 업그레이드
    [SerializeField] Button fillerButton;      // 충진기 업그레이드
    [SerializeField] Button decoTableButton;   // 장식 테이블 업그레이드

    [Header("팝업")]
    [SerializeField] PurchaseConfirmPopup popup;

    [Header("가격 설정")]
    [SerializeField] int loomPrice = 3000;
    [SerializeField] int fillerPrice = 5000;
    [SerializeField] int decoTablePrice = 8000;

    // 업그레이드 완료 여부
    bool loomUpgraded = false;
    bool fillerUpgraded = false;
    bool decoTableUpgraded = false;

    void Awake()
    {
        loomButton.onClick.AddListener(OnLoomClick);
        fillerButton.onClick.AddListener(OnFillerClick);
        decoTableButton.onClick.AddListener(OnDecoTableClick);
    }

    void OnLoomClick()
    {
        if (loomUpgraded)
        {
            Debug.Log("직조기 이미 업그레이드됨");
            return;
        }

        popup.ShowMessage(
            $"직조기 업그레이드 (가격 {loomPrice} G)\n진행하시겠습니까?",
            DoLoomUpgrade);
    }

    void OnFillerClick()
    {
        if (fillerUpgraded)
        {
            Debug.Log("충진기 이미 업그레이드됨");
            return;
        }

        popup.ShowMessage(
            $"충진기 업그레이드 (가격 {fillerPrice} G)\n진행하시겠습니까?",
            DoFillerUpgrade);
    }

    void OnDecoTableClick()
    {
        if (decoTableUpgraded)
        {
            Debug.Log("장식 테이블 이미 업그레이드됨");
            return;
        }

        popup.ShowMessage(
            $"장식 테이블 업그레이드 (가격 {decoTablePrice} G)\n진행하시겠습니까?",
            DoDecoTableUpgrade);
    }

    void DoLoomUpgrade()
    {
        // TODO: 코인 차감/효과 적용
        loomUpgraded = true;
        Debug.Log("직조기 업그레이드 완료!");
    }

    void DoFillerUpgrade()
    {
        fillerUpgraded = true;
        Debug.Log("충진기 업그레이드 완료!");
    }

    void DoDecoTableUpgrade()
    {
        decoTableUpgraded = true;
        Debug.Log("장식 테이블 업그레이드 완료!");
    }
}
