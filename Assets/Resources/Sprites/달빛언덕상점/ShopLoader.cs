using UnityEngine;

public class ShopLoader : MonoBehaviour
{
    [SerializeField] Transform contentRoot; // FlowLayoutGroup
    [SerializeField] ItemCard cardPrefab;
    [SerializeField] ItemData[] stock;       // 인스펙터 배열
    [SerializeField] PurchaseConfirmPopup popup;

    void Start()
    {
        foreach (var data in stock)
        {
            var card = Instantiate(cardPrefab, contentRoot);
            card.Init(data, OnBuyRequest);
        }
    }

    // 카드가 구매 버튼을 눌렀을 때 호출
    void OnBuyRequest(ItemCard card)
    {
        popup.Show(card, () => FinalizePurchase(card));
    }

    // YES 눌렀을 때
    void FinalizePurchase(ItemCard card)
    {
        if (card.IsRecruit)
        {
            Debug.Log($"유닛 고용! : {card.Data.displayName}");
            // ex) Instantiate(card.Data.recruitPrefab);
        }
        else
        {
            int cost = card.Data.price * card.Quantity;
            Debug.Log($"아이템 구매 : {card.Data.displayName} x{card.Quantity} , 비용 {cost}");
            // ex) 코인 차감, 인벤토리 추가 등
        }
    }
}
