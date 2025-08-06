using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemCard : MonoBehaviour
{
    [Header("필수 UI")]
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Button buyButton;
    [SerializeField] Image coinImage;

    [Header("수량 UI (없어도 OK)")]
    [SerializeField] GameObject qtyRow;     // 없으면 비워둬도 됨
    [SerializeField] Button incButton;
    [SerializeField] Button decButton;
    [SerializeField] TMP_Text qtyText;

    [Header("재화 사진")]
    public Sprite goldSprite;
    public Sprite moonrockSprite;

    public ItemData Data { get; private set; }
    public int Quantity { get; private set; } = 1;
    public bool IsRecruit { get; private set; }

    Action<ItemCard> onBuy;

    /// <summary>ShopLoader에서 Instantiate 후 바로 호출</summary>
    public void Init(ItemData data, Action<ItemCard> buyCallback)
    {
        Data = data;
        onBuy = buyCallback;
        IsRecruit = !data.useQuantity;

        // UI 세팅
        iconImage.sprite = data.icon;
        nameText.text = data.displayName;
        priceText.text = data.price.ToString("N0");
        coinImage.sprite = data.isGold ? goldSprite : moonrockSprite;

        // 기존 리스너 정리(풀링 대비)
        buyButton.onClick.RemoveAllListeners();
        if (incButton) incButton.onClick.RemoveAllListeners();
        if (decButton) decButton.onClick.RemoveAllListeners();

        // 수량 UI 처리
        bool hasQtyUI = data.useQuantity && qtyRow && incButton && decButton && qtyText;
        if (hasQtyUI)
        {
            qtyRow.SetActive(true);
            Quantity = Mathf.Clamp(data.defaultQty, data.minQty, data.maxQty);
            qtyText.text = Quantity.ToString();

            incButton.onClick.AddListener(() => ChangeQty(+1));
            decButton.onClick.AddListener(() => ChangeQty(-1));
        }
        else
        {
            if (qtyRow) qtyRow.SetActive(false);
            Quantity = 1;
        }

        buyButton.onClick.AddListener(OnBuyClicked);
    }

    void OnBuyClicked()
    {
        Debug.Log("[ItemCard] BuyClicked " + Data.displayName);
        onBuy?.Invoke(this);
    }

    void ChangeQty(int d)
    {
        Quantity = Mathf.Clamp(Quantity + d, Data.minQty, Data.maxQty);
        if (qtyText) qtyText.text = Quantity.ToString();
    }
}
