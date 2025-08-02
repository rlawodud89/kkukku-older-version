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
    private int energyLevel;
    private int energyPercent;

    private Dictionary<string, ItemScript> Items = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Blankets = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Snacks = new Dictionary<string, ItemScript>();
    private Dictionary<string, ItemScript> Interiors = new Dictionary<string, ItemScript>();
    private Dictionary<string, CustomerScript> Customers = new Dictionary<string, CustomerScript>();
    private Dictionary<string, WorkerScript> Workers = new Dictionary<string, WorkerScript>();
    private Dictionary<string, QuestScript> Quests = new Dictionary<string, QuestScript>();
    private Dictionary<string, LetterSciprt> Letters = new Dictionary<string, LetterSciprt>();

    private static float gameStartTime = 25200; // 오전 7시 (7 * 3600)
    private static float gameDuration = 1f; // 75초(1.25분)에 1시간 (30분에 24시간)
    private static int dayHours = 7;
    private static int eveningHours = 15;
    private static int nightHours = 0;

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
            User user = dbManager.Get_User();

            gold = user.gold;
            moonrock = user.moonrock;
            playTime = user.playTime;
            energyLevel = user.energyLevel;
            energyPercent = user.energyPercent;

            //LoadAllScriptableObjects();

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

        dbManager.Set_User(energyLevel, energyPercent, gold, moonrock, playTime);
    }

    private void LoadAllScriptableObjects()
    {
        //"...": addressable 라벨 이름
        Items = Addressables.LoadAssetsAsync<ItemScript>("item", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Blankets = Addressables.LoadAssetsAsync<ItemScript>("blanket", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Snacks = Addressables.LoadAssetsAsync<ItemScript>("snack", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Interiors = Addressables.LoadAssetsAsync<ItemScript>("interior", null)
                .WaitForCompletion()
                .ToDictionary(i => i.itemName);
        Customers = Addressables.LoadAssetsAsync<CustomerScript>("customer", null)
                .WaitForCompletion()
                .ToDictionary(i => i.customerName);
        Workers = Addressables.LoadAssetsAsync<WorkerScript>("worker", null)
                .WaitForCompletion()
                .ToDictionary(i => i.workerName);
        Quests = Addressables.LoadAssetsAsync<QuestScript>("quest", null)
                .WaitForCompletion()
                .ToDictionary(i => i.questName);
        Letters = Addressables.LoadAssetsAsync<LetterSciprt>("letter", null)
                .WaitForCompletion()
                .ToDictionary(i => i.letterName);
    }


    public int Get_Gold() { return gold; }
    public void Set_Gold(int gold) { this.gold = gold; }
    public void Change_Gold(int delta) { gold += delta; }

    public int Get_Moonrock() { return moonrock; }
    public void Set_Moonrock(int moonrock) { this.moonrock = moonrock; }
    public void Change_Moonrock(int delta) { moonrock += delta; }

    public int Get_EnergyLevel() { return energyLevel; }
    public void Set_EnergyLevel(int energyLevel) { this.energyLevel = energyLevel; }
    public void Change_EnergyLevel(int delta) { energyLevel += delta; }

    public int Get_EnergyPercent() { return energyPercent; }
    public void Set_EnergyPercent(int energyPercent) { this.energyPercent = energyPercent; }
    public void Change_EnergyPercent(int delta) { energyPercent += delta; }

    public int Get_Days() { return days; }
    public int Get_Hours() { return hours; }
    public int Get_Minutes() { return minutes; }


    public ItemScript Get_Item(string itemName) { return Items[itemName]; }
    public ItemScript Get_Random_Item()
    {
        int randomIdx = UnityEngine.Random.Range(0, Items.Count);
        var randomItem = Items.ElementAt(randomIdx);
        return randomItem.Value;
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
        int randomIdx = UnityEngine.Random.Range(0, Snacks.Count);
        var randomSnack = Snacks.ElementAt(randomIdx);
        return randomSnack.Value;
    }

    public ItemScript Get_Interior(string interiorName) { return Interiors[interiorName]; }
    public ItemScript Get_Random_Interior()
    {
        int randomIdx = UnityEngine.Random.Range(0, Interiors.Count);
        var randomInterior = Interiors.ElementAt(randomIdx);
        return randomInterior.Value;
    }

    public ItemScript Get_Random_InAll()
    {
        Func<ItemScript>[] funcs = new Func<ItemScript>[] {
            Get_Random_Item,
            Get_Random_Blanket,
            Get_Random_Snack,
            Get_Random_Interior
        };

        int random = UnityEngine.Random.Range(0, funcs.Length);
        return funcs[random]();
    }


    public CustomerScript Get_Customer(string customerName) { return Customers[customerName]; }
    public CustomerScript Get_Random_Customer()
    {
        int randomIdx = UnityEngine.Random.Range(0, Customers.Count);
        var randomCustomer = Customers.ElementAt(randomIdx);
        return randomCustomer.Value;
    }

    public WorkerScript Get_Worker(string workerName) { return Workers[workerName]; }

    public QuestScript Get_Quest(string questName) { return Quests[questName]; }
    public QuestScript Get_Random_Quest()
    {
        int randomIdx = UnityEngine.Random.Range(0, Quests.Count);
        var randomQuest = Quests.ElementAt(randomIdx);
        return randomQuest.Value;
    }

    public LetterSciprt Get_Letter(string letterName) { return Letters[letterName]; }
}