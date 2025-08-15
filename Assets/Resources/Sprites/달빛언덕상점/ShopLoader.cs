using System.Collections.Generic;
using UnityEngine;

public class ShopLoader : MonoBehaviour
{
    [SerializeField] Transform contentRoot; // FlowLayoutGroup
    [SerializeField] ItemCard cardPrefab;
    [SerializeField] List<ItemData> stock;       // 인스펙터 배열
    [SerializeField] PurchaseConfirmPopup popup;
    [SerializeField] ShopType shopType;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
        HashSet<string> uniqueList = new HashSet<string>();


        if (shopType == ShopType.SHOP_INTERIOR) // 가게 인테리어
        {
            while (stock.Count < 3)
            {
                InteriorScript interiorScript = gameManager.Get_Random_ShopInterior();
                if (uniqueList.Contains(interiorScript.interiorName)) continue;

                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = interiorScript.interiorName;
                itemData.icon = interiorScript.image;
                itemData.price = interiorScript.value;
                itemData.useQuantity = false; // 수량 X
                itemData.isGold = true; // 일반 재화 사용
                stock.Add(itemData);
                uniqueList.Add(itemData.displayName);
            }
        }
        else if (shopType == ShopType.ROOM_INTERIROR)
        {
            while (stock.Count < 3)
            {
                InteriorScript interiorScript = gameManager.Get_Random_RoomInterior();
                if (uniqueList.Contains(interiorScript.interiorName)) continue;

                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = interiorScript.interiorName;
                itemData.icon = interiorScript.image;
                itemData.price = interiorScript.value;
                itemData.isGold = true; // 일반 재화 사용
                stock.Add(itemData);
                uniqueList.Add(itemData.displayName);
            }
        }
        else if (shopType == ShopType.TILE) // 타일
        {
            while (stock.Count < 3)
            {
                InteriorScript interiorScript = gameManager.Get_Random_Tile();
                if (uniqueList.Contains(interiorScript.interiorName)) continue;

                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = interiorScript.interiorName;
                itemData.icon = interiorScript.image;
                itemData.price = interiorScript.value;
                itemData.useQuantity = false; // 수량 X
                itemData.isGold = true; // 일반 재화 사용
                stock.Add(itemData);
                uniqueList.Add(itemData.displayName);
            }
        }
        else if (shopType == ShopType.BLANKET) // 이불 디자인
        {
            while (stock.Count < 3)
            {
                ItemScript itemScript = gameManager.Get_Random_Blanket();
                if (uniqueList.Contains(itemScript.itemName)) continue;

                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.designValue;
                itemData.useQuantity = false; // 수량 X
                itemData.isGold = false; // 월석 사용
                stock.Add(itemData);
                uniqueList.Add(itemData.displayName);
            }
        }
        // 직원은 Inspector에서 설정

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
        if (card.IsRecruit) // 수량 필요없는 경우
        {
            Debug.Log($"디자인 ! : {card.Data.displayName}");

            if (shopType == ShopType.SHOP_INTERIOR) // 가게 인테리어 (수량 필요없이 겉만 바뀜)
            {
                if (card.Data.price > gameManager.Get_Gold())
                {
                    Debug.Log("재화 부족");
                    return;
                }

                if (gameManager.Add_InteriorItem(card.Data.displayName, 1)) // 없던 아이템이라 추가됐으면
                {
                    gameManager.Change_Gold(-card.Data.price);
                }
                else
                {
                    Debug.Log("이미 있는 인테리어이므로 추가 X");
                }
            }
            else if (shopType == ShopType.BLANKET) // 이불 디자인
            {
                if (card.Data.price > gameManager.Get_Moonrock())
                {
                    Debug.Log("월석 부족");
                    return;
                }

                if (gameManager.Add_BlanketDesign(card.Data.displayName)) // 없던 디자인이라 추가됐으면
                {
                    gameManager.Change_Moonrock(-card.Data.price);
                }
                else
                {
                    Debug.Log("이미 있는 디자인이므로 추가 X");
                }
            }
            else if (shopType == ShopType.TILE) // 타일
            {
                if (card.Data.price > gameManager.Get_Gold())
                {
                    Debug.Log("재화 부족");
                    return;
                }

                if (gameManager.Add_TileItem(card.Data.displayName)) // 없던 타일이라 추가됐으면
                {
                    gameManager.Change_Gold(-card.Data.price);
                }
                else
                {
                    Debug.Log("이미 있는 타일이므로 추가 X");
                }
            }
        }
        else
        {
            int cost = card.Data.price * card.Quantity;
            Debug.Log($"아이템 구매 : {card.Data.displayName} x{card.Quantity} , 비용 {cost}");

            // ex) 코인 차감, 인벤토리 추가 등

            if (card.Data.isGold) // 작업실 인테리어 (일반 재화 사용, 수량 O)
            {
                if (cost > gameManager.Get_Gold())
                {
                    Debug.Log("재화 부족");
                    return;
                }

                if (gameManager.Add_InteriorItem(card.Data.displayName, card.Quantity))
                {
                    gameManager.Change_Gold(-card.Data.price);
                }
            }
            else // 월석 사용 가게
            {
                if (cost > gameManager.Get_Moonrock())
                {
                    Debug.Log("월석 부족");
                    return;
                }

                if (shopType == ShopType.WORKER) // 직원
                {
                    gameManager.Add_InteriorItem(card.Data.displayName, card.Quantity);
                }
                else // 면, 솜, 장식
                {
                    gameManager.Add_InventoryItem(card.Data.displayName, card.Quantity);
                }

                gameManager.Change_Moonrock(-cost);
            }

        }
    }
}

public enum ShopType
{
    SHOP_INTERIOR,
    ROOM_INTERIROR,
    TILE,
    YARN,
    COTTON,
    DECO,
    BLANKET,
    WORKER
}