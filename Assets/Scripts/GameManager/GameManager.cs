using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Linq;
using System;

public enum NOW
{
    DAY,
    EVENING,
    NIGHT
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private DBManager dbManager;

    private int days;
    private int hours;
    private int minutes;
    private NOW nowTime;
    private float playTime;

    private int gold;
    private int moonrock;
    private int energy;

    private int designshopLevel;
    private int itemshopLevel;
    private int loomLevel;
    private int fillerLevel;
    private int decoLevel;

    private string endScene;
    private bool isOpen;

    private Dictionary<string, ItemScript> Materials = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Blankets = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Snacks = new Dictionary<string, ItemScript>();

    private Dictionary<string, InteriorScript> Shop_Interiors = new Dictionary<string, InteriorScript>();
    private Dictionary<string, InteriorScript> Room_Interiors = new Dictionary<string, InteriorScript>();
    private Dictionary<string, InteriorScript> Workers = new Dictionary<string, InteriorScript>();
    private Dictionary<string, InteriorScript> Tiles = new Dictionary<string, InteriorScript>();

    private Dictionary<string, CustomerScript> Customers = new Dictionary<string, CustomerScript>();
    private Dictionary<string, QuestScript> Quests = new Dictionary<string, QuestScript>();
    private Dictionary<string, LetterSciprt> Letters = new Dictionary<string, LetterSciprt>();

    private static float gameStartTime = 25200; // 오전 7시 (7 * 3600)
    private static float gameDuration = 75f; // 75초(1.25분)에 1시간 (30분에 24시간)
    private static int dayHours = 7;
    private static int eveningHours = 15;
    private static int nightHours = 0;
    private static float oneEnergyLevel = 3844;

    //싱글톤 패턴 위한 private 생성자, 인스턴스 반환 정적 메서드
    private GameManager() { }
    public static GameManager getInstance() { return instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            dbManager = DBManager.getInstance();
            //dbManager.InitDB();

            User user = dbManager.Get_User();
            energy = user.energy;
            gold = user.gold;
            moonrock = user.moonrock;
            designshopLevel = user.designshopLevel;
            itemshopLevel = user.itemshopLevel;
            loomLevel = user.loomLevel;
            fillerLevel = user.fillerLevel;
            decoLevel = user.decoLevel;
            playTime = user.playTime;
            endScene = user.endScene;
            isOpen = user.isOpen;

            LoadAllScriptableObjects();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        playTime += (Time.deltaTime / gameDuration) * 3600;
        hours = (int)(playTime / 3600) % 24;
        minutes = (int)(playTime % 3600) / 60;
        days = (int)(playTime / (3600 * 24)) + 1; // Day1부터 시작하므로 +1

        if (hours == nightHours) // 밤 진입
        {
            playTime += gameStartTime; // 0시 0분 되면 아침 시간(7시 0분)으로 넘어감
            nowTime = NOW.NIGHT;
            Debug.Log("Days:" + days);
        }
        else if (hours == dayHours) // 아침 진입
        {
            nowTime = NOW.DAY;
        }
        else if (hours == eveningHours) // 저녁 진입
        {
            nowTime = NOW.EVENING;
        }

        dbManager.Update_User(energy, gold, moonrock, playTime);
    }

    private void LoadAllScriptableObjects()
    {
        //"...": addressable 라벨 이름
        Materials = Addressables.LoadAssetsAsync<ItemScript>("material", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Blankets = Addressables.LoadAssetsAsync<ItemScript>("blanket", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Snacks = Addressables.LoadAssetsAsync<ItemScript>("snack", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Shop_Interiors = Addressables.LoadAssetsAsync<InteriorScript>("shop_interior", null)
                .WaitForCompletion()
                .ToDictionary(i => i.interiorName);
        Room_Interiors = Addressables.LoadAssetsAsync<InteriorScript>("room_interior", null)
                .WaitForCompletion()
                .ToDictionary(i => i.interiorName);
        Workers = Addressables.LoadAssetsAsync<InteriorScript>("worker", null)
                .WaitForCompletion()
                .ToDictionary(i => i.interiorName);
        /*Tiles = Addressables.LoadAssetsAsync<InteriorScript>("tile", null)
                .WaitForCompletion()
                .ToDictionary(i => i.interiorName);
        Customers = Addressables.LoadAssetsAsync<CustomerScript>("customer", null)
                .WaitForCompletion()
                .ToDictionary(i => i.customerName);
        Quests = Addressables.LoadAssetsAsync<QuestScript>("quest", null)
                .WaitForCompletion()
                .ToDictionary(i => i.questName);
        Letters = Addressables.LoadAssetsAsync<LetterSciprt>("letter", null)
                .WaitForCompletion()
                .ToDictionary(i => i.letterName);*/
    }

    private int Get_RandomLevel()
    {
        // 높은 레벨이 덜 선택되도록 가중치 설정 
        int weight1 = 60;
        int weight2 = 30;
        int weight3 = 10;
        int totalWeight = weight1 + weight2 + weight3;
        int rand = UnityEngine.Random.Range(1, totalWeight + 1);
        if (rand <= weight1) return 1;
        else if (rand <= weight1 + weight2) return 2;
        else return 3;
    }


    public int Get_Gold() { return gold; }
    public void Set_Gold(int gold) { this.gold = gold; }
    public void Change_Gold(int delta) { gold += delta; }

    public int Get_Moonrock() { return moonrock; }
    public void Set_Moonrock(int moonrock) { this.moonrock = moonrock; }
    public void Change_Moonrock(int delta) { moonrock += delta; }

    public int Get_EnergyLevel() { return (int)(energy / oneEnergyLevel); }
    public float Get_EnergyPercent() { return ((energy % oneEnergyLevel) / oneEnergyLevel) * 100; }
    public void Set_Energy(int energy) { this.energy = energy; }
    public void Change_Energy(int delta) { energy += delta; }

    public int Get_Days() { return days; }
    public int Get_Hours() { return hours; }
    public int Get_Minutes() { return minutes; }

    public int Get_DesignShopLevel() { return designshopLevel; }
    public void Set_DesignShopLevel(int level)
    {
        designshopLevel = level;
        dbManager.Update_DesginShopLevel(designshopLevel);
    }
    public void Change_DesignShopLevel(int delta)
    {
        designshopLevel += delta;
        dbManager.Update_DesginShopLevel(designshopLevel);
    }

    public int Get_ItemShopLevel() { return itemshopLevel; }
    public void Set_ItemShopLevel(int level)
    {
        itemshopLevel = level;
        dbManager.Update_ItemShopLevel(itemshopLevel);
    }
    public void Change_ItemShopLevel(int delta)
    {
        itemshopLevel += delta;
        dbManager.Update_ItemShopLevel(itemshopLevel);
    }

    public int Get_LoomLevel() { return loomLevel; }
    public void Set_LoomLevel(int level)
    {
        loomLevel += level;
        dbManager.Update_LoomLevel(loomLevel);
    }
    public void Change_LoomLevel(int delta)
    {
        loomLevel += delta;
        dbManager.Update_LoomLevel(loomLevel);
    }

    public int Get_FillerLevel() { return fillerLevel; }
    public void Set_FillerLevel(int level)
    {
        fillerLevel = level;
        dbManager.Update_FillerLevel(fillerLevel);
    }
    public void Change_FillerLevel(int delta)
    {
        fillerLevel += delta;
        dbManager.Update_FillerLevel(fillerLevel);
    }

    public int Get_DecoLevel() { return decoLevel; }
    public void Set_DecoLevel(int level)
    {
        decoLevel = level;
        dbManager.Update_DecoLevel(decoLevel);
    }
    public void Change_DecoLevel(int delta)
    {
        decoLevel += delta;
        dbManager.Update_DecoLevel(decoLevel);
    }

    public bool Get_IsOpen() { return isOpen; }
    public void Set_IsOpen(bool isOpen)
    {
        this.isOpen = isOpen;
        dbManager.Update_IsOpen(this.isOpen);
    }


    public ItemScript Get_Material(string materialName) { return Materials[materialName]; }
    public ItemScript Get_Random_Material()
    {
        int randomIdx = UnityEngine.Random.Range(0, Materials.Count);
        var randomMaterial = Materials.ElementAt(randomIdx);
        return randomMaterial.Value;
    }

    public ItemScript Get_Blanket(string blanketName) { return Blankets[blanketName]; }
    public ItemScript Get_Random_Blanket()
    {
        int randomIdx = UnityEngine.Random.Range(0, Blankets.Count);
        var randomBlanket = Blankets.ElementAt(randomIdx);
        return randomBlanket.Value;
    }
    


    public ItemScript Get_Snack(string snackName) { return Snacks[snackName]; }
    public ItemScript Get_Random_Snack()
    {
        // 간식 레벨 선택 
        int randomlevel = Get_RandomLevel();

        // 해당하는 레벨의 간식 나올 때까지 랜덤 선택
        int randomIdx;
        KeyValuePair<string, ItemScript> randomSnank;
        do
        {
            randomIdx = UnityEngine.Random.Range(0, Snacks.Count);
            randomSnank = Snacks.ElementAt(randomIdx);
        } while (randomSnank.Value.level != randomlevel);

        return randomSnank.Value;
    }

    public InteriorScript Get_ShopInterior(string interiorName) { return Shop_Interiors[interiorName]; }
    public InteriorScript Get_Random_ShopInterior()
    {
        int randomIdx = UnityEngine.Random.Range(0, Shop_Interiors.Count);
        var randomInterior = Shop_Interiors.ElementAt(randomIdx);
        return randomInterior.Value;
    }

    public InteriorScript Get_RoomInterior(string interiorName) { return Room_Interiors[interiorName]; }
    public InteriorScript Get_Random_RoomInterior()
    {
        int randomIdx = UnityEngine.Random.Range(0, Room_Interiors.Count);
        var randomInterior = Room_Interiors.ElementAt(randomIdx);
        return randomInterior.Value;
    }

    public InteriorScript Get_Tile(string tileName) { return Tiles[tileName]; }
    public InteriorScript Get_Random_Tile()
    {
        int randomIdx = UnityEngine.Random.Range(0, Tiles.Count);
        var randomTile = Tiles.ElementAt(randomIdx);
        return randomTile.Value;
    }

    public CustomerScript Get_Customer(string customerName) { return Customers[customerName]; }
    public CustomerScript Get_Random_Customer()
    {
        int randomIdx = UnityEngine.Random.Range(0, Customers.Count);
        var randomCustomer = Customers.ElementAt(randomIdx);
        return randomCustomer.Value;
    }

    public QuestScript Get_Quest(string questName) { return Quests[questName]; }
    public QuestScript Get_Random_Quest()
    {
        int randomIdx = UnityEngine.Random.Range(0, Quests.Count);
        var randomQuest = Quests.ElementAt(randomIdx);
        return randomQuest.Value;
    }
    public LetterSciprt Get_Letter(string letterName) { return Letters[letterName]; }

    public ItemScript Get_InventoryItem(string itemName)
    {
        if (Materials.ContainsKey(itemName)) return Materials[itemName];
        else if (Blankets.ContainsKey(itemName)) return Blankets[itemName];
        else if (Snacks.ContainsKey(itemName)) return Snacks[itemName];
        else return null;
    }

    public InteriorScript Get_InteriorItem(string itemName)
    {
        if (Room_Interiors.ContainsKey(itemName)) return Room_Interiors[itemName];
        else if (Shop_Interiors.ContainsKey(itemName)) return Shop_Interiors[itemName];
        else if (Workers.ContainsKey(itemName)) return Workers[itemName];
        else return null;
    }


    public void Add_InventoryItem(string itemName, int count)
    {
        if (count <= 0) return;

        if (dbManager.Have_Inventory(itemName))
        {
            dbManager.Change_InventoryItem_Count(itemName, count);
        }
        else
        {
            ItemScript itemScript = Get_InventoryItem(itemName);
            if (itemScript != null) dbManager.Insert_InventoryItem(itemName, itemScript.itemType, count);
        }
    }

    public bool Add_BlanketDesign(string blanketName)
    {
        if (dbManager.Have_Design(blanketName)) return false;
        else
        {
            dbManager.Insert_Design(blanketName);
            return true;
        }
    }

    public bool Add_InteriorItem(string interiorName, int count)
    {
        if (count <= 0) return false;

        InteriorScript interiorScript = Get_InteriorItem(interiorName);

        if (interiorScript.interiorType == InteriorType.SHOP_INTERIOR)
        {
            if (dbManager.Have_InteriorItem(interiorName)) return false;
            else count = 1;
        }

        dbManager.Insert_InteriorItem(interiorName, interiorScript.interiorType, count);
        return true;

    }

    public bool Add_TileItem(string tileName)
    {
        if (dbManager.Have_InteriorItem(tileName)) return false;
        else
        {
            InteriorScript tileScript = Get_Tile(tileName);
            dbManager.Insert_Tile(tileName, tileScript.interiorType);
            return true;
        }
    }


    public List<(ItemScript item, int count)> Get_Material_Inventory()
    {
        List<Inventory> inven = dbManager.Select_Material();
        List<(ItemScript item, int count)> result = new List<(ItemScript item, int count)>();

        foreach (Inventory i in inven)
        {
            result.Add((Get_Material(i.itemName), i.count));
        }

        return result;
    }


    public List<(ItemScript item, int count)> Get_Blanket_Inventory()
    {
        List<Inventory> inven = dbManager.Select_Blanket();
        List<(ItemScript item, int count)> result = new List<(ItemScript item, int count)>();

        foreach (Inventory i in inven)
        {
            result.Add((Get_Blanket(i.itemName), i.count));
        }

        return result;
    }

    public List<(ItemScript item, int count)> Get_Snack_Inventory()
    {
        List<Inventory> inven = dbManager.Select_Snack();
        List<(ItemScript item, int count)> result = new List<(ItemScript item, int count)>();

        foreach (Inventory i in inven)
        {
            result.Add((Get_Snack(i.itemName), i.count));
        }

        return result;
    }

    public List<(InteriorScript item, int count)> Get_RoomInterior_Inventory()
    {
        List<(string itemName, int count)> inven = dbManager.Select_RoomInterior();
        List<(InteriorScript item, int count)> result = new List<(InteriorScript item, int count)>();

        foreach ((string itemName, int count) i in inven)
        {
            result.Add((Get_RoomInterior(i.itemName), i.count));
        }

        return result;
    }


    public bool Use_InventoryItem(string itemName, int count)
    {
        if (count <= 0) return false;

        if (!dbManager.Have_Inventory(itemName)) return false;

        if (dbManager.Change_InventoryItem_Count(itemName, -count)) return true;
        else return false;

    }

    public bool Use_RoomInteriorItem(string interiorName, int x, int y)
    {
        if(!dbManager.Have_InteriorItem(interiorName)) return false;

        if (dbManager.Set_InteriorItem(interiorName, x, y)) return true;
        else return false;
    }
}