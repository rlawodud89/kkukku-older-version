using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopController : MonoBehaviour
{
    [SerializeField] Button materialUpgradeButton;  // 재료 상점 업글
    [SerializeField] Button designUpgradeButton;    // 디자인 상점 업글
    [SerializeField] PurchaseConfirmPopup popup;

    [Header("레벨 텍스트들")]
    [SerializeField] TMP_Text itemshoplevelText;
    [SerializeField] TMP_Text designshoplevelText;
    

    [Header("가격 / 상태")]
    [SerializeField] int materialUpgradePrice = 5000;
    [SerializeField] int designUpgradePrice = 8000;

    private GameManager gameManager;
    bool materialUpgraded = false;
    bool designUpgraded = false;

    void Awake()
    {
        gameManager = GameManager.getInstance();
        materialUpgradeButton.onClick.AddListener(OnMaterialUpgradeClick);
        designUpgradeButton.onClick.AddListener(OnDesignUpgradeClick);

        itemshoplevelText.text = "Lv. " + gameManager.Get_ItemShopLevel();
        designshoplevelText.text = "Lv. " + gameManager.Get_DesignShopLevel();
    }

    void OnMaterialUpgradeClick()
    {
        if (!CanBuy(materialUpgradePrice))
        {
            Debug.Log("월석 부족");
            return;
        }

        popup.ShowMessage(
            $"재료 상점 업그레이드 (가격 {materialUpgradePrice} G)\n구매하시겠습니까?",
            () => DoMaterialUpgrade());
    }

    void OnDesignUpgradeClick()
    {
        if (!CanBuy(designUpgradePrice))
        {
            Debug.Log("월석 부족");
            return;
        }

        popup.ShowMessage(
            $"디자인 상점 업그레이드 (가격 {designUpgradePrice} G)\n구매하시겠습니까?",
            () => DoDesignUpgrade());
    }

    void DoMaterialUpgrade()
    {
        gameManager.Change_Moonrock(-materialUpgradePrice);
        gameManager.Change_ItemShopLevel(1);
        itemshoplevelText.text = "Lv. " + gameManager.Get_ItemShopLevel().ToString();
        Debug.Log("재료 상점 업그레이드 완료!");
    }

    void DoDesignUpgrade()
    {
        gameManager.Change_Moonrock(-designUpgradePrice);
        gameManager.Change_DesignShopLevel(1);
        designshoplevelText.text = "Lv. " + gameManager.Get_DesignShopLevel().ToString();
        Debug.Log("디자인 상점 업그레이드 완료!");
    }

    private bool CanBuy(int value)
    {
        if (value <= gameManager.Get_Moonrock()) return true;
        else return false;
    }
}
