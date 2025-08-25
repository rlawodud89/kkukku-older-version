using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Linq;
using System;
using static UnityEditor.MaterialProperty;
using UnityEngine.SceneManagement;

public enum BgType
{
    DAY,
    EVENING,
    NIGHT
}


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private DBManager dbManager;


    // 사용자 정보
    private int days;
    private int hours;
    private int minutes;
    private BgType bgTime;
    private float playTime;

    private int gold;
    private int moonrock;
    private int energy;
    private int todayGold;
    private int todayMoonrock;
    private int todayEnergy;

    private int designshopLevel;
    private int itemshopLevel;
    private int loomLevel;
    private int fillerLevel;
    private int decoLevel;

    private string endScene;
    private bool isOpen;
    private float bgSound;
    private float effectSound;


    // ScriptableObject Dictionary
    private Dictionary<string, ItemScript> Materials = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Blankets = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Snacks = new Dictionary<string, ItemScript>();

    private Dictionary<string, ItemScript> Yarns = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Cottons = new Dictionary<string, ItemScript>();
    private Dictionary<string, string> Map_Yarn_to_Cotton = new Dictionary<string, string>();
    private Dictionary<string, string> Map_Cotton_to_Blanket = new Dictionary<string, string>();

    private Dictionary<string, InteriorScript> Shop_Interiors = new Dictionary<string, InteriorScript>();
    private Dictionary<string, InteriorScript> Room_Interiors = new Dictionary<string, InteriorScript>();
    private Dictionary<string, InteriorScript> Workers = new Dictionary<string, InteriorScript>();
    private Dictionary<string, InteriorScript> Tiles = new Dictionary<string, InteriorScript>();

    private Dictionary<string, CustomerScript> Customers = new Dictionary<string, CustomerScript>();
    private Dictionary<string, QuestSO> Quests = new Dictionary<string, QuestSO>();
    private Dictionary<string, LetterScript> Letters = new Dictionary<string, LetterScript>();


    // GameManager에서 사용하는 상수
    private static float gameStartTime = 25200; // 오전 7시 (7 * 3600)
    private static float gameDuration = 75f; // 75초(1.25분)에 1시간 (30분에 24시간)
    private static int dayHours = 7;
    private static int eveningHours = 15;
    private static int nightHours = 22;
    private static int endHours = 0;
    private static int shopCloseHours = 18;
    private static float oneEnergyLevel = 3844;
    private static float dbSaveTimer = 0f;    // DB 저장 주기 타이머
    private static float dbSaveInterval = 1f; // 1초마다 저장 (원하는 값으로 변경 가능)


    // 시간에 따라 배경 바뀌게 하고, 하루 정리 패널 뜨게 하는 이벤트
    public event Action<BgType> OnBgTimeChanged;
    public event Action OnDayEnded;
    public bool isDayEndPanel;
    public event Action OnshopCloseHours;

    // 가게에서 게임 매니저 값에 따라 이불장, 표지판 바뀔 수 있도록 하는 이벤트
    public event Action<bool> OnOpenChanged;
    public event Action<string, int> OnBlanketInvenChanged; // string: 변경된 이불 이름, int: 인벤토리에 추가/삭제된 수량
    public event Action<int, string, int> OnTableBlanketChanged; // 테이블 ID, 변경된 이불 이름, 이불장에 추가/삭제된 수량
    public event Action<int> OnTableInteriorChanged; // 테이블 ID, 변경된 이불 이름, 이불장에 추가/삭제된 수량

    // 타일 변경 시 적용되도록 하는 이벤트
    public event Action<TilePosType, InteriorScript> OnTileChanged;

    // 상점 레벨 변경 시 적용되도록 하는 이벤트
    public event Action<int> OnItemShopLevelChanged;
    public event Action<int> OnDesignShopLevelChanged;


    //싱글톤 패턴 위한 private 생성자, 인스턴스 반환 정적 메서드
    private GameManager() { }
    public static GameManager getInstance() { return instance; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return; // 기존 인스턴스 유지, 새 객체는 바로 리턴
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        dbManager = DBManager.getInstance();
        //dbManager.InitDB();

        User user = dbManager.Get_User();
        energy = user.energy;
        gold = user.gold;
        moonrock = user.moonrock;
        todayEnergy = user.todayEnergy;
        todayGold = user.todayGold;
        todayMoonrock = user.todayMoonrock;
        designshopLevel = user.designshopLevel;
        itemshopLevel = user.itemshopLevel;
        loomLevel = user.loomLevel;
        fillerLevel = user.fillerLevel;
        decoLevel = user.decoLevel;
        playTime = user.playTime;
        endScene = user.endScene;
        isOpen = user.isOpen;
        bgSound = user.bgSound;
        effectSound = user.effectSound;

        LoadAllScriptableObjects();
        LoadBgTime();

    }

    void Update()
    {
        if (isDayEndPanel) return;

        playTime += (Time.deltaTime / gameDuration) * 3600;
        hours = (int)(playTime / 3600) % 24;
        minutes = (int)(playTime % 3600) / 60;
        days = (int)(playTime / (3600 * 24)) + 1; // Day1부터 시작하므로 +1

        if (hours == endHours) // 하루 끝
        {
            Reset_Store_ContentItem();
            OnDayEnded?.Invoke();
            isDayEndPanel = true;
        }
        else if (hours == dayHours) // 아침 진입
        {
            bgTime = BgType.DAY;
            OnBgTimeChanged?.Invoke(bgTime);
        }
        else if (hours == eveningHours) // 저녁 진입
        {
            bgTime = BgType.EVENING;
            OnBgTimeChanged?.Invoke(bgTime);
        }
        else if (hours == nightHours)
        {
            bgTime = BgType.NIGHT;
            OnBgTimeChanged?.Invoke(bgTime);
        }
        else if (hours == shopCloseHours)
        {
            if (isOpen)
            {
                Set_IsOpen(false);
                OnshopCloseHours?.Invoke();
            }
        }

        dbSaveTimer += Time.deltaTime;
        if (dbSaveTimer >= dbSaveInterval)
        {
            dbManager.Update_PlayTime(playTime);
            dbSaveTimer = 0f; // 타이머 초기화
        }
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
        Yarns = Addressables.LoadAssetsAsync<ItemScript>("yarn", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Cottons = Addressables.LoadAssetsAsync<ItemScript>("cotton", null)
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
        Tiles = Addressables.LoadAssetsAsync<InteriorScript>("tile", null)
                .WaitForCompletion()
                .ToDictionary(i => i.interiorName);
        Quests = Addressables.LoadAssetsAsync<QuestSO>("quest", null)
                .WaitForCompletion()
                .ToDictionary(i => i.questTitle);
        /*
        Letters = Addressables.LoadAssetsAsync<LetterScript>("letter", null)
                .WaitForCompletion()
                .ToDictionary(i => i.letterName);
        */


        foreach (var blanket in Blankets)
        {
            ItemScript value = blanket.Value;
            if (value.yarnName == "" || value.cottonName == "") continue;

            Map_Yarn_to_Cotton.Add(value.yarnName, value.cottonName);
            Map_Cotton_to_Blanket.Add(value.cottonName, value.itemName);
        }
    }

    private void LoadBgTime()
    {
        hours = (int)(playTime / 3600) % 24;

        if (hours >= endHours && hours < dayHours)
        {
            bgTime = BgType.NIGHT;
            OnBgTimeChanged?.Invoke(bgTime);
            OnDayEnded?.Invoke();
        }
        else if (hours >= nightHours)
        {
            bgTime = BgType.NIGHT;
            OnBgTimeChanged?.Invoke(bgTime);
        }
        else if (hours >= eveningHours)
        {
            bgTime = BgType.EVENING;
            OnBgTimeChanged?.Invoke(bgTime);
        }
        else if (hours >= dayHours)
        {
            bgTime = BgType.DAY;
            OnBgTimeChanged?.Invoke(bgTime);
        }

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



    // 사용자 정보 관련 메서드

    public int Get_Gold() { return gold; }
    public void Change_Gold(int delta)
    {
        gold += delta;
        dbManager.Update_Gold(gold);

        if (delta > 0)
        {
            todayGold += delta;
            dbManager.Update_TodayGold(todayGold);
        }
    }

    public int Get_Moonrock() { return moonrock; }
    public void Change_Moonrock(int delta)
    {
        moonrock += delta;
        dbManager.Update_Moonrock(moonrock);

        if (delta > 0)
        {
            todayMoonrock += delta;
            dbManager.Update_TodayMoonrock(todayMoonrock);
        }
    }

    public int Get_EnergyLevel() { return (int)(energy / oneEnergyLevel); }
    public float Get_EnergyPercent() { return ((energy % oneEnergyLevel) / oneEnergyLevel) * 100; }
    public void Change_Energy(int delta)
    {
        if (delta <= 0) return;

        energy += delta;
        dbManager.Update_Energy(energy);
        todayEnergy += delta;
        dbManager.Update_TodayEnergy(todayEnergy);
    }

    public int Get_TodayGold() { return todayGold; }
    public int Get_TodayMoonrock() { return todayMoonrock; }
    public int Get_TodayEnergy() { return todayEnergy; }
    public void Reset_User_Todays()
    {
        dbManager.Reset_User_Todays();
    }

    public int Get_Days() { return days; }
    public int Get_Hours() { return hours; }
    public int Get_Minutes() { return minutes; }
    public BgType Get_BgTime() { return bgTime; }

    public int Get_DesignShopLevel() { return designshopLevel; }
    public void Change_DesignShopLevel(int delta)
    {
        designshopLevel += delta;
        dbManager.Update_DesginShopLevel(designshopLevel);
        OnDesignShopLevelChanged?.Invoke(designshopLevel);
    }

    public int Get_ItemShopLevel() { return itemshopLevel; }
    public void Change_ItemShopLevel(int delta)
    {
        itemshopLevel += delta;
        dbManager.Update_ItemShopLevel(itemshopLevel);
        OnItemShopLevelChanged?.Invoke(itemshopLevel);
    }

    public int Get_LoomLevel() { return loomLevel; }
    public void Change_LoomLevel(int delta)
    {
        loomLevel += delta;
        dbManager.Update_LoomLevel(loomLevel);
    }

    public int Get_FillerLevel() { return fillerLevel; }
    public void Change_FillerLevel(int delta)
    {
        fillerLevel += delta;
        dbManager.Update_FillerLevel(fillerLevel);
    }

    public int Get_DecoLevel() { return decoLevel; }
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
        OnOpenChanged?.Invoke(isOpen);
    }

    public float Get_BgSound() { return bgSound; }
    public void Set_BgSound(float bgSound)
    {
        this.bgSound = bgSound;
        dbManager.Update_BgSound(this.bgSound);
    }

    public float Get_EffectSound() { return effectSound; }
    public void Set_EffectSound(float effectSound)
    {
        this.effectSound = effectSound;
        dbManager.Update_EffectSound(this.effectSound);
    }

    public string Get_EndScene() { return endScene; }
    public void Set_EndScene(string endScene)
    {
        this.endScene = endScene;
        dbManager.Update_EndScene(this.endScene);
    }


    public void Go_Next_Days()
    {
        isDayEndPanel = false;
        playTime += gameStartTime; // 0시 0분 되면 아침 시간(7시 0분)으로 넘어감
        dbManager.Update_PlayTime(playTime);

        todayGold = 0;
        todayMoonrock = 0;
        todayEnergy = 0;
        Reset_User_Todays();

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Work_Shop")
        {
            Set_EndScene("Work_Shop");
            SceneManager.LoadScene("Work_Shop");
        }
    }



    // ScritableObject 관련 getter, 랜덤 요소 하나 받아오는 getter

    public ItemScript Get_Yarn(string yarnName) { return Yarns[yarnName]; }
    public ItemScript Get_Cotton(string cottonName) { return Cottons[cottonName]; }

    public ItemScript Get_Material(string materialName) { return Materials[materialName]; }
    public ItemScript Get_Random_Material()
    {
        int randomlevel = Get_RandomLevel();

        int randomIdx;
        KeyValuePair<string, ItemScript> randomMaterial;
        do
        {
            randomIdx = UnityEngine.Random.Range(0, Materials.Count);
            randomMaterial = Materials.ElementAt(randomIdx);
        } while (randomMaterial.Value.level != randomlevel);

        return randomMaterial.Value;
    }

    public ItemScript Get_Blanket(string blanketName) { return Blankets[blanketName]; }
    public ItemScript Get_Random_Blanket()
    {
        KeyValuePair<string, ItemScript> randomBlanket;

        int randomIdx;
        do
        {
            randomIdx = UnityEngine.Random.Range(0, Blankets.Count);
            randomBlanket = Blankets.ElementAt(randomIdx);
        } while (randomBlanket.Value.itemName == "기본이불" || randomBlanket.Value.isSpecial);

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
        KeyValuePair<string, InteriorScript> randomInterior;

        int randomIdx;
        do
        {
            randomIdx = UnityEngine.Random.Range(0, Shop_Interiors.Count);
            randomInterior = Shop_Interiors.ElementAt(randomIdx);
        } while (randomInterior.Value.interiorName == "나무벽장" || randomInterior.Value.interiorName == "나무진열장");

        return randomInterior.Value;
    }

    public InteriorScript Get_RoomInterior(string interiorName) { return Room_Interiors[interiorName]; }
    public InteriorScript Get_Random_RoomInterior()
    {
        KeyValuePair<string, InteriorScript> randomInterior;

        int randomIdx;
        do
        {
            randomIdx = UnityEngine.Random.Range(0, Room_Interiors.Count);
            randomInterior = Room_Interiors.ElementAt(randomIdx);
        } while (randomInterior.Value.interiorName == "특별제작대");

        return randomInterior.Value;
    }

    public InteriorScript Get_Tile(string tileName) { return Tiles[tileName]; }
    public InteriorScript Get_Random_Tile()
    {
        KeyValuePair<string, InteriorScript> randomTile;

        int randomIdx;
        do
        {
            randomIdx = UnityEngine.Random.Range(0, Tiles.Count);
            randomTile = Tiles.ElementAt(randomIdx);
        } while (randomTile.Value.interiorName == "나무벽" || randomTile.Value.interiorName == "나무바닥");

        return randomTile.Value;
    }

    public CustomerScript Get_Customer(string customerName) { return Customers[customerName]; }
    public CustomerScript Get_Random_Customer()
    {
        int randomIdx = UnityEngine.Random.Range(0, Customers.Count);
        var randomCustomer = Customers.ElementAt(randomIdx);
        return randomCustomer.Value;
    }

    public QuestSO Get_Quest(string questName) { return Quests[questName]; }
    public QuestSO Get_Random_Quest()
    {
        int randomIdx = UnityEngine.Random.Range(0, Quests.Count);
        var randomQuest = Quests.ElementAt(randomIdx);
        QuestSO quest = randomQuest.Value;
        quest.questProcess = 0;
        quest.isCompleted = false;
        quest.getReward = false;
        return quest;
    }
    public LetterScript Get_Letter(string letterName) { return Letters[letterName]; }

    public ItemScript Get_InventoryItem(string itemName)
    {
        if (itemName == null || itemName == "") return null;

        if (Materials.ContainsKey(itemName)) return Materials[itemName];
        else if (Blankets.ContainsKey(itemName)) return Blankets[itemName];
        else if (Snacks.ContainsKey(itemName)) return Snacks[itemName];
        else if (Yarns.ContainsKey(itemName)) return Yarns[itemName];
        else if (Cottons.ContainsKey(itemName)) return Cottons[itemName];
        else return null;
    }

    public InteriorScript Get_InteriorItem(string itemName)
    {
        if (itemName == null || itemName == "") return null;

        if (Room_Interiors.ContainsKey(itemName)) return Room_Interiors[itemName];
        else if (Shop_Interiors.ContainsKey(itemName)) return Shop_Interiors[itemName];
        else if (Workers.ContainsKey(itemName)) return Workers[itemName];
        else if (Tiles.ContainsKey(itemName)) return Tiles[itemName];
        else return null;
    }

    public Sprite GetMaterialImage(RecipeEntry entry)
    {
        ItemScript item = Get_InventoryItem(entry.itemName);
        if (item == null) return null;
        return item.image;
    }

    public ItemScript Blanket_to_Yarn(string blanketName)
    {
        ItemScript blanket = Get_Blanket(blanketName);
        return Get_Yarn(blanket.yarnName);
    }

    public ItemScript Yarn_to_Cotton(string yarnName)
    {
        if (!Map_Yarn_to_Cotton.ContainsKey(yarnName)) return null;

        string cottonName = Map_Yarn_to_Cotton[yarnName];
        return Get_Cotton(cottonName);
    }

    public ItemScript Cotton_to_Blanket(string cottonName)
    {
        if (!Map_Cotton_to_Blanket.ContainsKey(cottonName)) return null;

        string blanketName = Map_Cotton_to_Blanket[cottonName];
        return Get_Blanket(blanketName);
    }



    // DB에서 데이터 받아오거나, 저장하는 메서드

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

        if (Get_InventoryItem(itemName).itemType == ItemType.BLANKET)
        {
            OnBlanketInvenChanged?.Invoke(itemName, count); // 가게에 인벤토리 이불량 늘었다고 알림
        }
    }

    public bool Use_InventoryItem(string itemName, int count)
    {
        if (count <= 0) return false;

        if (!dbManager.Have_Inventory(itemName)) return false;

        if (dbManager.Change_InventoryItem_Count(itemName, -count))
        {
            if (Get_InventoryItem(itemName).itemType == ItemType.BLANKET)
            {
                OnBlanketInvenChanged?.Invoke(itemName, -count); // 가게에 이불 사용해서 재고 줄었다고 변경되었다고 알림
            }

            return true;
        }
        else return false;
    }

    public int Count_InventoryItem(string itemName)
    {
        Inventory item = dbManager.Select_InventoryItem(itemName);
        if (item == null) return 0;
        else return item.count;
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

    public List<ItemScript> Get_Current_BlanketDesign()
    {
        List<Design> designs = dbManager.Select_Design();
        List<ItemScript> list = new List<ItemScript>();

        foreach (Design d in designs)
        {
            list.Add(Get_InventoryItem(d.blanketName));
        }

        return list;
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
            dbManager.Insert_New_Tile(tileName, tileScript.interiorType);
            return true;
        }
    }

    public List<(ItemScript, int count)> Get_Yarn_Inventory()
    {
        List<Inventory> inven = dbManager.Select_Yarn();
        List<(ItemScript item, int count)> result = new List<(ItemScript item, int count)>();

        foreach (Inventory i in inven)
        {
            result.Add((Get_Yarn(i.itemName), i.count));
        }

        return result;
    }

    public List<(ItemScript, int count)> Get_Cotton_Inventory()
    {
        List<Inventory> inven = dbManager.Select_Cotton();
        List<(ItemScript item, int count)> result = new List<(ItemScript item, int count)>();

        foreach (Inventory i in inven)
        {
            result.Add((Get_Cotton(i.itemName), i.count));
        }

        return result;
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
            if (Get_Blanket(i.itemName).isSpecial) continue;
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
        List<(string itemName, int count)> inven = dbManager.Select_RoomInterior_Inventory();
        List<(InteriorScript item, int count)> result = new List<(InteriorScript item, int count)>();

        foreach ((string itemName, int count) i in inven)
        {
            result.Add((Get_InteriorItem(i.itemName), i.count));
        }

        return result;
    }

    public void Add_Table_Blanket(int tableID, string blanketName, int count)
    {
        if (count <= 0) return;

        if (dbManager.Have_Table_Blanket(tableID, blanketName))
        {
            dbManager.Change_TableBlanket_Count(tableID, blanketName, count);

        }
        else
        {
            ItemScript itemScript = Get_InventoryItem(blanketName);
            if (itemScript != null) dbManager.Insert_TableBlanket(tableID, blanketName, count);
        }

        OnTableBlanketChanged?.Invoke(tableID, blanketName, count); // 이불장엔 이불량 늘었다고 알림 
    }

    public bool Use_Table_Blanket(int tableID, string blanketName, int count)
    {
        if (count <= 0) return false;

        if (!dbManager.Have_Table_Blanket(tableID, blanketName)) return false;

        if (dbManager.Change_TableBlanket_Count(tableID, blanketName, -count))
        {
            OnTableBlanketChanged?.Invoke(tableID, blanketName, -count); // 이불장에 이불 줄었다고 알림

            return true;
        }
        else return false;
    }

    public List<(ItemScript blanket, int count)> Get_Table_Blanket(int tableID)
    {
        List<ShopTable> list = dbManager.Select_Table_Blanket(tableID);
        List<(ItemScript blanket, int count)> result = new List<(ItemScript blanket, int count)>();

        foreach (ShopTable st in list)
        {
            result.Add((Get_Blanket(st.blanketName), st.count));
        }
        return result;
    }

    public (InteriorScript table, bool isFull) Get_Current_Table(int tableID)
    {
        WorkShop workShop = dbManager.Select_WorkShop(tableID);
        return (Get_InteriorItem(workShop.tableName), dbManager.Any_Table_Blanket(tableID));
    }

    public int Use_RandomOne_BlanketInTable(int tableID) // 랜덤으로 해당 테이블에 있는 이불 한 개 선택, 선택된 이불의 가격 반환
    {
        if (!dbManager.Any_Table_Blanket(tableID)) return 0;

        // 랜덤으로 한 개의 이불 선택
        List<ShopTable> list = dbManager.Select_Table_Blanket(tableID);
        int randomIdx = UnityEngine.Random.Range(0, list.Count);
        ShopTable randomBlanket = list.ElementAt(randomIdx);
        ItemScript blanketScript = Get_Blanket(randomBlanket.blanketName);

        // 한 개 테이블에서 가져간 거 DB에 저장
        if (Use_Table_Blanket(tableID, blanketScript.itemName, 1))
        {
            return blanketScript.value;
        }

        else return 0;
    }

    public bool Use_RoomInteriorItem(string interiorName, float x, float y)
    {
        if (!dbManager.Set_InteriorItem(interiorName, x, y)) return false;

        InteriorScript interiorScript = Get_InteriorItem(interiorName);
        if (interiorScript.interiorType == InteriorType.WORKER)
        {
            return dbManager.Insert_Worker(interiorName, x, y);
        }
        else
        {
            return true;
        }
    }

    public bool Move_RoomInteriorItem(float beforeX, float beforeY, float afterX, float afterY)
    {
        return dbManager.Change_InteriorItem_Pos(beforeX, beforeY, afterX, afterY);
    }

    public bool Back_RoomInteriorItem(float x, float y)
    {
        return dbManager.NotSet_InteriorItem(x, y); // 직원인 경우, WorkRoom 데이터 삭제하는 기능도 구현 O
    }

    public List<(InteriorScript item, float x, float y)> Get_Current_RoomInterior()
    {
        List<Interior> interiors = dbManager.Select_Current_RoomInterior();
        List<(InteriorScript item, float x, float y)> list = new List<(InteriorScript item, float x, float y)>();

        foreach (Interior i in interiors)
        {
            list.Add((Get_InteriorItem(i.interiorName), i.x, i.y));
        }

        return list;
    }

    public Sprite Get_Current_Tile(TilePosType tilePosType)
    {
        Tile tile = dbManager.Select_Tile(tilePosType);
        InteriorScript tileScript = Get_Tile(tile.tileName);
        return tileScript.image;
    }

    public void Set_Current_Tile(TilePosType tilePosType, string tileName)
    {
        dbManager.Update_Tile(tilePosType, tileName);
        OnTileChanged?.Invoke(tilePosType, Get_InteriorItem(tileName));
    }

    public List<InteriorScript> Get_FloorTile_Inventory()
    {
        List<Interior> floorTile = dbManager.Select_FloorTile_Inventory();
        List<InteriorScript> list = new List<InteriorScript>();

        foreach (Interior i in floorTile)
        {
            list.Add(Get_InteriorItem(i.interiorName));
        }

        return list;
    }

    public List<InteriorScript> Get_WallTile_Inventory()
    {
        List<Interior> floorTile = dbManager.Select_WallTile_Inventory();
        List<InteriorScript> list = new List<InteriorScript>();

        foreach (Interior i in floorTile)
        {
            list.Add(Get_InteriorItem(i.interiorName));
        }

        return list;
    }

    public List<InteriorScript> Get_ShopInterior_Inventory()
    {
        List<Interior> shopInterior = dbManager.Select_ShopInterior_Inventory();
        List<InteriorScript> list = new List<InteriorScript>();

        foreach (Interior i in shopInterior)
        {
            list.Add(Get_InteriorItem(i.interiorName));
        }

        return list;
    }

    public void Set_ShopTableInterior(int tableID, string interiorName)
    {
        dbManager.Update_ShopTableInterior(tableID, interiorName);
        OnTableInteriorChanged?.Invoke(tableID);
    }

    public List<QuestSO> Get_Current_Quest()
    {
        List<QuestBox> quests = dbManager.Select_All_Quest();
        List<QuestSO> list = new List<QuestSO>();

        foreach (QuestBox q in quests)
        {
            QuestSO quest = Get_Quest(q.questName);
            quest.questProcess = q.process;
            quest.isCompleted = q.isCompleted;
            quest.getReward = q.getReward;

            list.Add(quest);
        }

        return list;
    }

    public void Add_Quest(string questName)
    {
        dbManager.Insert_Quest(questName);
    }

    public void Remove_Quest(string questName)
    {
        dbManager.Delete_Quest(questName);
    }

    public void Set_Quest_Process(string questName, int process)
    {
        dbManager.Update_Quest_Process(questName, process);
    }

    public void Set_Quest_IsCompleted(string questName, bool isCompleted)
    {
        dbManager.Update_Quest_IsCompleted(questName, isCompleted);
    }

    public void Set_Quest_GetReward(string questName, bool getReward)
    {
        dbManager.Update_Quest_GetReward(questName, getReward);
    }


    public (int workerID, int stamina, ItemScript workItem, float workingPercent) Get_Worker_Info(float x, float y)
    {
        int workerID = dbManager.Select_Worker_ID(x, y);
        WorkRoom worker = dbManager.Select_Worker_Info(workerID);

        return (workerID, worker.stamina, Get_InventoryItem(worker.workItem), worker.workingPercent);
    }

    public void Change_Worker_Stamina(int workerID, int delta)
    {
        dbManager.Change_Worker_Stamina(workerID, delta);
    }

    public void Set_Worker_workingItem(int workerID, string workingItemName)
    {
        dbManager.Update_Worker_workingItem(workerID, workingItemName);
    }

    public void Set_Worker_WorkingPercent(int workerID, float workingPercent)
    {
        dbManager.Update_Worker_WorkingPercent(workerID, workingPercent);
    }

    public List<LetterScript> Get_Current_Letter()
    {
        List<LetterBox> letters = dbManager.Select_Current_Letter();
        List<LetterScript> list = new List<LetterScript>();

        foreach (LetterBox i in letters)
        {
            list.Add(Get_Letter(i.letterName));
        }

        return list;
    }

    public void Add_Letter(string letterName)
    {
        dbManager.Insert_Letter(letterName);
    }

    public void Remove_Letter(string letterName)
    {
        dbManager.Delete_Letter(letterName);
    }

    public List<ItemScript> Get_ItemStore_ContentItem(StoreType storeType)
    {
        if (storeType == StoreType.SHOP_INTERIOR || storeType == StoreType.ROOM_INTERIROR
            || storeType == StoreType.TILE || storeType == StoreType.WORKER)
            return null;

        List<StoreItem> storeItems = dbManager.Select_StoreItem(storeType);
        List<ItemScript> list = new List<ItemScript>();

        foreach (StoreItem item in storeItems)
        {
            list.Add(Get_InventoryItem(item.itemName));
        }

        return list;

    }

    public List<InteriorScript> Get_InteriorStore_ContentItem(StoreType storeType)
    {
        if (storeType == StoreType.YARN || storeType == StoreType.COTTON
            || storeType == StoreType.DECO || storeType == StoreType.BLANKET)
            return null;

        List<StoreItem> storeItems = dbManager.Select_StoreItem(storeType);
        List<InteriorScript> list = new List<InteriorScript>();

        foreach (StoreItem item in storeItems)
        {
            list.Add(Get_InteriorItem(item.itemName));
        }

        return list;
    }

    public bool Add_Store_ContentItem(StoreType storeType, string itemName)
    {
        if (dbManager.Have_StoreItem(itemName)) return false;

        dbManager.Insert_StoreItem(storeType, itemName);
        return true;
    }

    public void Reset_Store_ContentItem()
    {
        dbManager.Delete_All_StoreItem();

        HashSet<string> uniqueList = new HashSet<string>();

        // 가게 인테리어
        while (uniqueList.Count < 3)
        {
            InteriorScript interiorScript = Get_Random_ShopInterior();
            if (uniqueList.Contains(interiorScript.interiorName)) continue;

            Add_Store_ContentItem(StoreType.SHOP_INTERIOR, interiorScript.interiorName);
            uniqueList.Add(interiorScript.interiorName);
        }
        uniqueList.Clear();

        // 작업실 인테리어
        while (uniqueList.Count < 3)
        {
            InteriorScript interiorScript = Get_Random_RoomInterior();
            if (uniqueList.Contains(interiorScript.interiorName)) continue;

            Add_Store_ContentItem(StoreType.ROOM_INTERIROR, interiorScript.interiorName);
            uniqueList.Add(interiorScript.interiorName);
        }
        uniqueList.Clear();

        // 타일
        while (uniqueList.Count < 3)
        {
            InteriorScript interiorScript = Get_Random_Tile();
            if (uniqueList.Contains(interiorScript.interiorName)) continue;

            Add_Store_ContentItem(StoreType.TILE, interiorScript.interiorName);
            uniqueList.Add(interiorScript.interiorName);
        }
        uniqueList.Clear();

        // 이불 디자인
        while (uniqueList.Count < designshopLevel + 1)
        {
            ItemScript itemScript = Get_Random_Blanket();
            if (uniqueList.Contains(itemScript.itemName)) continue;

            Add_Store_ContentItem(StoreType.BLANKET, itemScript.itemName);
            uniqueList.Add(itemScript.itemName);
        }
    }


}