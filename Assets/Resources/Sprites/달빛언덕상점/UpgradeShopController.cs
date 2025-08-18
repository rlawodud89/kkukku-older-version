using System;
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
    [SerializeField] int levelLimit = 3;

    private GameManager gameManager;
    bool materialUpgraded = false;
    bool designUpgraded = false;
    public Action<SpeechType> speechType;

    void Awake()
    {
        gameManager = GameManager.getInstance();
        materialUpgradeButton.onClick.AddListener(OnMaterialUpgradeClick);
        designUpgradeButton.onClick.AddListener(OnDesignUpgradeClick);

        itemshoplevelText.text = "Lv. " + gameManager.Get_ItemShopLevel();
        designshoplevelText.text = "Lv. " + gameManager.Get_DesignShopLevel();

        Init_UpgradePrice();
    }

    private void Init_UpgradePrice()
    {
        if (gameManager.Get_ItemShopLevel() == 2) materialUpgradePrice += 3000;
        if (gameManager.Get_DesignShopLevel() == 2) designUpgradePrice += 3000;
    }

    void OnMaterialUpgradeClick()
    {
        if (!CanBuy(materialUpgradePrice))
        {
            Debug.Log("월석 부족");
            speechType?.Invoke(SpeechType.Lack);
            return;
        }

        if (gameManager.Get_ItemShopLevel() >= levelLimit)
        {
            speechType?.Invoke(SpeechType.Limit);
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
            speechType?.Invoke(SpeechType.Lack);
            return;
        }

        if (gameManager.Get_DesignShopLevel() >= levelLimit)
        {
            speechType?.Invoke(SpeechType.Limit);
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
        speechType?.Invoke(SpeechType.Trigger);
        Init_UpgradePrice();
    }

    void DoDesignUpgrade()
    {
        gameManager.Change_Moonrock(-designUpgradePrice);
        gameManager.Change_DesignShopLevel(1);
        designshoplevelText.text = "Lv. " + gameManager.Get_DesignShopLevel().ToString();
        speechType?.Invoke(SpeechType.Trigger);
        Init_UpgradePrice();
    }

    private bool CanBuy(int value)
    {
        if (value <= gameManager.Get_Moonrock()) return true;
        else return false;
    }
}
