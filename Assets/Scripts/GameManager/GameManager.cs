using UnityEngine;
using System.Collections.Generic;
//using SQLite;
using System.IO;
using Unity.VisualScripting;
//using UnityEngine.AddressableAssets;
using System.Linq;

enum NOW
{
    DAY,
    EVENING,
    NIGHT
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    private int days;
    private int hours;
    private int minutes;
    private NOW nowTime;
    private float playTime;

    private int gold;
    private int moonrock;
    private int energyLevel;
    private int energyPercent;

    //private Dictionary<string, Item> Items = new Dictionary<string, Item>();

    private static float gameStartTime = 25200; // 오전 7시 (7 * 3600)
    private static float gameDuration = 1f; // 75초(1.25분)에 1시간 (30분에 24시간)
    private static int dayHours = 7;
    private static int eveningHours = 15;
    private static int nightHours = 0;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //테스트용 초기화
            gold = 100;
            moonrock = 100;
            playTime = 0;
            energyLevel = 0;
            energyPercent = 0;

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
    }

    //private void LoadAllScriptableObjects()
    //{
    //    Items = Addressables.LoadAssetsAsync<Item>("Item", null)
    //            .WaitForCompletion()
    //            .ToDictionary(i => i.itemName);

    //    Debug.Log(Items["졸린베리덤불"].value);
    //}


    public int Get_Gold()
    {
        return gold;
    }

    public void Set_Gold(int gold)
    {
        this.gold = gold;
    }

    public void Change_Gold(int delta)
    {
        gold += delta;
    }

    public int Get_Moonrock()
    {
        return moonrock;
    }

    public void Set_Moonrock(int moonrock)
    {
        this.moonrock = moonrock;
    }

    public void Change_Moonrock(int delta)
    {
        moonrock += delta;
    }

    public int Get_EnergyLevel()
    {
        return energyLevel;
    }

    public void Set_EnergyLevel(int energyLevel)
    {
        this.energyLevel = energyLevel;
    }

    public void Change_EnergyLevel(int delta)
    {
        energyLevel += delta;
    }

    public int Get_EnergyPercent()
    {
        return energyPercent;
    }

    public void Set_EnergyPercent(int energyPercent)
    {
        this.energyPercent = energyPercent;
    }

    public void Change_EnergyPercent(int delta)
    {
        energyPercent += delta;
    }

    public int Get_Days()
    {
        return days;
    }

    public int Get_Hours()
    {
        return hours;
    }

    public int Get_Minutes()
    {
        return minutes;
    }
}