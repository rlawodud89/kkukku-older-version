using System.Collections.Generic;
using UnityEngine;

public class ShopLoader : MonoBehaviour
{
    [SerializeField] Transform contentRoot; // FlowLayoutGroup
    [SerializeField] ItemCard cardPrefab;
    [SerializeField] List<ItemData> stock;       // 인스펙터 배열
    [SerializeField] PurchaseConfirmPopup popup;
    [SerializeField] bool isShop; // 가게 패널인 경우 true, 작업실 패널인 경우 false

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
        HashSet<string> uniqueList = new HashSet<string>();

        if (isShop) // 가게 패널인 경우
        {
            while(stock.Count < 3)
            {
                ItemScript itemScript = gameManager.Get_Random_ShopInterior();
                if (uniqueList.Contains(itemScript.itemName)) continue;

                ItemData itemData = new ItemData();
                itemData.displayName = itemScript.name;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                stock.Add(itemData);
                uniqueList.Add(itemData.displayName);
            }
        }
        else // 작업실 패널인 경우
        {
            while (stock.Count < 3)
            {
                ItemScript itemScript = gameManager.Get_Random_RoomInterior();
                if (uniqueList.Contains(itemScript.itemName)) continue;

                ItemData itemData = new ItemData();
                itemData.displayName = itemScript.name;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                stock.Add(itemData);
                uniqueList.Add(itemData.displayName);
            }
        }

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

            if(cost > gameManager.Get_Gold())
            {
                Debug.Log("잔액 부족");
                return;
            }

            gameManager.Change_Gold(-cost);
            gameManager.Add_to_Inventory(card.Data.displayName, card.Quantity);
        }
    }
}
