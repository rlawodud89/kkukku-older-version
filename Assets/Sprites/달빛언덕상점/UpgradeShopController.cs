using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopController : MonoBehaviour
{
    [SerializeField] Button materialUpgradeButton;  // 재료 상점 업글
    [SerializeField] Button designUpgradeButton;    // 디자인 상점 업글
    [SerializeField] PurchaseConfirmPopup popup;

    [Header("가격 / 상태")]
    [SerializeField] int materialUpgradePrice = 5000;
    [SerializeField] int designUpgradePrice = 8000;

    bool materialUpgraded = false;
    bool designUpgraded = false;

    void Awake()
    {
        materialUpgradeButton.onClick.AddListener(OnMaterialUpgradeClick);
        designUpgradeButton.onClick.AddListener(OnDesignUpgradeClick);
    }

    void OnMaterialUpgradeClick()
    {
        if (materialUpgraded)
        {
            Debug.Log("이미 재료 상점 업그레이드 완료");
            return;
        }

        popup.ShowMessage(
            $"재료 상점 업그레이드 (가격 {materialUpgradePrice} G)\n구매하시겠습니까?",
            () => DoMaterialUpgrade());
    }

    void OnDesignUpgradeClick()
    {
        if (designUpgraded)
        {
            Debug.Log("이미 디자인 상점 업그레이드 완료");
            return;
        }

        popup.ShowMessage(
            $"디자인 상점 업그레이드 (가격 {designUpgradePrice} G)\n구매하시겠습니까?",
            () => DoDesignUpgrade());
    }

    void DoMaterialUpgrade()
    {
        // 코인 차감 / 기능 열기 등 실제 로직
        Debug.Log("재료 상점 업그레이드 완료!");
        materialUpgraded = true;
    }

    void DoDesignUpgrade()
    {
        Debug.Log("디자인 상점 업그레이드 완료!");
        designUpgraded = true;
    }
}
