using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SQLite4Unity3d;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;


public class DBManager
{
    private static DBManager instance = new DBManager();
    private static string dbPath = "Assets/StreamingAssets/comforter_shop.db";
    private static string testdbPath = "Assets/StreamingAssets/comforter_shop_test.db";
    private static SQLiteConnection conn = new SQLiteConnection(dbPath);
    private static SQLiteConnection testconn = new SQLiteConnection(testdbPath);
    private static string userName = "user";

    //싱글톤 패턴 위한 private 생성자, 인스턴스 반환 정적 메서드
    private DBManager() { }
    public static DBManager getInstance() { return instance; }

    public void InitDB()
    {
        testconn.CreateTable<User>();
        testconn.CreateTable<Inventory>();
        testconn.CreateTable<WorkShop>();
        testconn.CreateTable<ShopTable>();
        testconn.CreateTable<WorkRoom>();
        testconn.CreateTable<QuestBox>();
        testconn.CreateTable<LetterBox>();

        User user = new User();
        user.name = "user";
        user.energyLevel = 1;
        user.energyPercent = 0;
        user.gold = 1000;
        user.moonrock = 1000;
        user.playTime = 0;

        testconn.Insert(user);
    }

    public User Get_User()
    {
        return testconn.Find<User>(userName); //지정한 이름(기본키)으로 찾기
    }

    public void Set_User(int energyLevel, int energyPercent, int gold, int moonrock, float playTime)
    {
        User user = testconn.Find<User>(userName);
        user.energyLevel = energyLevel;
        user.energyPercent = energyPercent;
        user.gold = gold;
        user.moonrock = moonrock;
        user.playTime = playTime;

        testconn.Update(user);
    }

    
}
