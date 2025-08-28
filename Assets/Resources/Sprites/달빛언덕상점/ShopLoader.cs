using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShopLoader : MonoBehaviour
{
    [SerializeField] Transform contentRoot; // FlowLayoutGroup
    [SerializeField] ItemCard cardPrefab;
    [SerializeField] List<ItemData> stock;       // 인스펙터 배열
    [SerializeField] PurchaseConfirmPopup popup;
    [SerializeField] StoreType shopType;
    public Action<SpeechType> speechTrigger;

    private GameManager gameManager;
    private int designshopLevel;
    private int itemshopLevel;

    private List<InteriorScript> shopInteriors;
    private List<InteriorScript> roomInteriors;
    private List<InteriorScript> tiles;
    private List<ItemScript> blankets;

    void Start()
    {
        gameManager = GameManager.getInstance();
        designshopLevel = gameManager.Get_DesignShopLevel();
        itemshopLevel = gameManager.Get_ItemShopLevel();

        gameManager.OnItemShopLevelChanged += ChangeItemShopLevel;
        gameManager.OnDesignShopLevelChanged += ChangeDesignShopLevel;
        gameManager.OnDayEnded += InitContent;

        shopInteriors = new List<InteriorScript>();
        roomInteriors = new List<InteriorScript>();
        tiles = new List<InteriorScript>();
        blankets = new List<ItemScript>();

        InitContent();
    }

    private void InitContent()
    {
        if (shopType != StoreType.WORKER) stock.Clear();
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }

        if (shopType == StoreType.SHOP_INTERIOR) // 가게 인테리어
        {
            shopInteriors.Clear();
            shopInteriors = gameManager.Get_InteriorStore_ContentItem(shopType);

            foreach (InteriorScript interiorScript in shopInteriors)
            {
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = interiorScript.interiorName;
                itemData.icon = interiorScript.image;
                itemData.price = interiorScript.value;
                itemData.useQuantity = false; // 수량 X
                itemData.isGold = true; // 일반 재화 사용
                stock.Add(itemData);
            }
        }
        else if (shopType == StoreType.ROOM_INTERIROR)
        {
            roomInteriors.Clear();
            roomInteriors = gameManager.Get_InteriorStore_ContentItem(shopType);

            foreach (InteriorScript interiorScript in roomInteriors)
            {
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = interiorScript.interiorName;
                itemData.icon = interiorScript.image;
                itemData.price = interiorScript.value;
                itemData.isGold = true; // 일반 재화 사용
                stock.Add(itemData);
            }
        }
        else if (shopType == StoreType.TILE) // 타일
        {
            tiles.Clear();
            tiles = gameManager.Get_InteriorStore_ContentItem(shopType);

            foreach (InteriorScript interiorScript in tiles)
            {
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = interiorScript.interiorName;
                itemData.icon = interiorScript.image;
                itemData.price = interiorScript.value;
                itemData.useQuantity = false; // 수량 X
                itemData.isGold = true; // 일반 재화 사용
                stock.Add(itemData);
            }
        }
        else if (shopType == StoreType.BLANKET) // 이불 디자인
        {
            blankets.Clear();
            blankets = gameManager.Get_ItemStore_ContentItem(shopType);

            foreach (ItemScript itemScript in blankets)
            {
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.designValue;
                itemData.useQuantity = false; // 수량 X
                itemData.isGold = false; // 월석 사용
                stock.Add(itemData);
            }
        }
        else if (shopType == StoreType.YARN)
        {
            // 1단계
            ItemScript itemScript1 = gameManager.Get_Material("꿈실");
            ItemData itemData1 = ScriptableObject.CreateInstance<ItemData>();
            itemData1.displayName = itemScript1.itemName;
            itemData1.icon = itemScript1.image;
            itemData1.price = itemScript1.value;
            itemData1.isGold = false;
            stock.Add(itemData1);

            if (itemshopLevel >= 2)
            {
                ItemScript itemScript2 = gameManager.Get_Material("별빛꿈실");
                ItemData itemData2 = ScriptableObject.CreateInstance<ItemData>();
                itemData2.displayName = itemScript2.itemName;
                itemData2.icon = itemScript2.image;
                itemData2.price = itemScript2.value;
                itemData2.isGold = false;
                stock.Add(itemData2);
            }
            if (itemshopLevel >= 3)
            {
                ItemScript itemScript3 = gameManager.Get_Material("은하꿈실");
                ItemData itemData3 = ScriptableObject.CreateInstance<ItemData>();
                itemData3.displayName = itemScript3.itemName;
                itemData3.icon = itemScript3.image;
                itemData3.price = itemScript3.value;
                itemData3.isGold = false;
                stock.Add(itemData3);
            }
        }
        else if (shopType == StoreType.COTTON)
        {
            // 1단계
            ItemScript itemScript1 = gameManager.Get_Material("운무솜");
            ItemData itemData1 = ScriptableObject.CreateInstance<ItemData>();
            itemData1.displayName = itemScript1.itemName;
            itemData1.icon = itemScript1.image;
            itemData1.price = itemScript1.value;
            itemData1.isGold = false;
            stock.Add(itemData1);

            if (itemshopLevel >= 2)
            {
                ItemScript itemScript2 = gameManager.Get_Material("햇빛운무솜");
                ItemData itemData2 = ScriptableObject.CreateInstance<ItemData>();
                itemData2.displayName = itemScript2.itemName;
                itemData2.icon = itemScript2.image;
                itemData2.price = itemScript2.value;
                itemData2.isGold = false;
                stock.Add(itemData2);
            }
            if (itemshopLevel >= 3)
            {
                ItemScript itemScript3 = gameManager.Get_Material("천공운무솜");
                ItemData itemData3 = ScriptableObject.CreateInstance<ItemData>();
                itemData3.displayName = itemScript3.itemName;
                itemData3.icon = itemScript3.image;
                itemData3.price = itemScript3.value;
                itemData3.isGold = false;
                stock.Add(itemData3);
            }
        }
        else if (shopType == StoreType.DECO)
        {
            // 1단계
            ItemScript itemScript1 = gameManager.Get_Material("달조각");
            ItemData itemData1 = ScriptableObject.CreateInstance<ItemData>();
            itemData1.displayName = itemScript1.itemName;
            itemData1.icon = itemScript1.image;
            itemData1.price = itemScript1.value;
            itemData1.isGold = false;
            stock.Add(itemData1);

            if (itemshopLevel >= 2)
            {
                ItemScript itemScript2 = gameManager.Get_Material("은빛달조각");
                ItemData itemData2 = ScriptableObject.CreateInstance<ItemData>();
                itemData2.displayName = itemScript2.itemName;
                itemData2.icon = itemScript2.image;
                itemData2.price = itemScript2.value;
                itemData2.isGold = false;
                stock.Add(itemData2);
            }
            if (itemshopLevel >= 3)
            {
                ItemScript itemScript3 = gameManager.Get_Material("청야달조각");
                ItemData itemData3 = ScriptableObject.CreateInstance<ItemData>();
                itemData3.displayName = itemScript3.itemName;
                itemData3.icon = itemScript3.image;
                itemData3.price = itemScript3.value;
                itemData3.isGold = false;
                stock.Add(itemData3);
            }
        }
        // 직원은 Inspector에서 설정

        foreach (var data in stock)
        {
            var card = Instantiate(cardPrefab, contentRoot);
            card.Init(data, OnBuyRequest);
        }
    }


    private void ChangeItemShopLevel(int itemshopLevel)
    {
        this.itemshopLevel = itemshopLevel;
        if (shopType == StoreType.YARN)
        {
            // 1단계는 이미 Inspector에서 넣어놓음

            if (itemshopLevel == 2)
            {
                ItemScript itemScript = gameManager.Get_Material("별빛꿈실");
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                itemData.isGold = false;
                stock.Add(itemData);
            }
            if (itemshopLevel == 3)
            {
                ItemScript itemScript = gameManager.Get_Material("은하꿈실");
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                itemData.isGold = false;
                stock.Add(itemData);
            }

            var card = Instantiate(cardPrefab, contentRoot);
            card.Init(stock.Last(), OnBuyRequest);
        }
        else if (shopType == StoreType.COTTON)
        {
            // 1단계는 이미 Inspector에서 넣어놓음

            if (itemshopLevel == 2)
            {
                ItemScript itemScript = gameManager.Get_Material("햇빛운무솜");
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                itemData.isGold = false;
                stock.Add(itemData);
            }
            if (itemshopLevel == 3)
            {
                ItemScript itemScript = gameManager.Get_Material("천공운무솜");
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                itemData.isGold = false;
                stock.Add(itemData);
            }

            var card = Instantiate(cardPrefab, contentRoot);
            card.Init(stock.Last(), OnBuyRequest);
        }
        else if (shopType == StoreType.DECO)
        {
            // 1단계는 이미 Inspector에서 넣어놓음

            if (itemshopLevel == 2)
            {
                ItemScript itemScript = gameManager.Get_Material("은빛달조각");
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                itemData.isGold = false;
                stock.Add(itemData);
            }
            if (itemshopLevel == 3)
            {
                ItemScript itemScript = gameManager.Get_Material("청야달조각");
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.displayName = itemScript.itemName;
                itemData.icon = itemScript.image;
                itemData.price = itemScript.value;
                itemData.isGold = false;
                stock.Add(itemData);
            }

            var card = Instantiate(cardPrefab, contentRoot);
            card.Init(stock.Last(), OnBuyRequest);
        }
    }

    private void ChangeDesignShopLevel(int designshopLevel)
    {
        if (shopType == StoreType.BLANKET)
        {
            ItemScript itemScript;
            while (true)
            {
                itemScript = gameManager.Get_Random_Blanket();
                if (gameManager.Add_Store_ContentItem(shopType, itemScript.itemName)) break;
            }

            ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
            itemData.displayName = itemScript.itemName;
            itemData.icon = itemScript.image;
            itemData.price = itemScript.designValue;
            itemData.useQuantity = false; // 수량 X
            itemData.isGold = false; // 월석 사용
            stock.Add(itemData);

            var card = Instantiate(cardPrefab, contentRoot);
            card.Init(stock.Last(), OnBuyRequest);
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

            if (shopType == StoreType.SHOP_INTERIOR) // 가게 인테리어 (수량 필요없이 겉만 바뀜)
            {
                if (card.Data.price > gameManager.Get_Gold())
                {
                    Debug.Log("재화 부족");
                    speechTrigger?.Invoke(SpeechType.Lack);
                    return;
                }

                if (gameManager.Add_InteriorItem(card.Data.displayName, 1)) // 없던 아이템이라 추가됐으면
                {
                    gameManager.Change_Gold(-card.Data.price);
                    speechTrigger?.Invoke(SpeechType.Trigger);
                    AddQuestProcess.Instance.AddProcessToQuest("인테리어 아이템 구매하기");
                }
                else
                {
                    Debug.Log("이미 있는 인테리어이므로 추가 X");
                    speechTrigger?.Invoke(SpeechType.Have);
                }
            }
            else if (shopType == StoreType.BLANKET) // 이불 디자인
            {
                if (card.Data.price > gameManager.Get_Moonrock())
                {
                    Debug.Log("월석 부족");
                    speechTrigger?.Invoke(SpeechType.Lack);
                    return;
                }

                if (gameManager.Add_BlanketDesign(card.Data.displayName)) // 없던 디자인이라 추가됐으면
                {
                    gameManager.Change_Moonrock(-card.Data.price);
                    speechTrigger?.Invoke(SpeechType.Trigger);
                }
                else
                {
                    Debug.Log("이미 있는 디자인이므로 추가 X");
                    speechTrigger?.Invoke(SpeechType.Have);
                }
            }
            else if (shopType == StoreType.TILE) // 타일
            {
                if (card.Data.price > gameManager.Get_Gold())
                {
                    Debug.Log("재화 부족");
                    speechTrigger?.Invoke(SpeechType.Lack);
                    return;
                }

                if (gameManager.Add_TileItem(card.Data.displayName)) // 없던 타일이라 추가됐으면
                {
                    gameManager.Change_Gold(-card.Data.price);
                    speechTrigger?.Invoke(SpeechType.Trigger);
                    AddQuestProcess.Instance.AddProcessToQuest("인테리어 아이템 구매하기");
                }
                else
                {
                    Debug.Log("이미 있는 타일이므로 추가 X");
                    speechTrigger?.Invoke(SpeechType.Have);
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
                    speechTrigger?.Invoke(SpeechType.Lack);
                    return;
                }

                if (gameManager.Add_InteriorItem(card.Data.displayName, card.Quantity))
                {
                    gameManager.Change_Gold(-card.Data.price);
                    speechTrigger?.Invoke(SpeechType.Trigger);
                    AddQuestProcess.Instance.AddProcessToQuest("인테리어 아이템 구매하기");
                }
            }
            else // 월석 사용 가게
            {
                if (cost > gameManager.Get_Moonrock())
                {
                    Debug.Log("월석 부족");
                    speechTrigger?.Invoke(SpeechType.Lack);
                    return;
                }

                if (shopType == StoreType.WORKER) // 직원
                {
                    gameManager.Add_InteriorItem(card.Data.displayName, card.Quantity);
                }
                else // 면, 솜, 장식
                {
                    gameManager.Add_InventoryItem(card.Data.displayName, card.Quantity);
                }

                gameManager.Change_Moonrock(-cost);
                speechTrigger?.Invoke(SpeechType.Trigger);
            }

        }
    }
}