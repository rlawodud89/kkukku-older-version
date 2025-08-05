using System;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.Linq;


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
        testconn.CreateTable<Design>();
        testconn.CreateTable<WorkShop>();
        testconn.CreateTable<ShopTable>();
        testconn.CreateTable<WorkRoom>();
        testconn.CreateTable<Interior>();
        testconn.CreateTable<Tile>();
        testconn.CreateTable<QuestBox>();
        testconn.CreateTable<LetterBox>();

        User user = new User();
        user.name = "user";
        user.energy = 0;
        user.gold = 1000;
        user.moonrock = 1000;
        user.playTime = 0;
        user.designshopLevel = 1;
        user.itemshopLevel = 1;
        user.loomLevel = 1;
        user.fillerLevel = 1;
        user.decoLevel = 1;
        user.endScene = "Work_Shop";
        user.isOpen = false;

        testconn.Insert(user);

        // TODO: 처음에 기본으로 주는 아이템 저장
    }

    public User Get_User()
    {
        return testconn.Find<User>(userName); //지정한 이름(기본키)으로 찾기
    }

    public void Update_User(int energy, int gold, int moonrock, float playTime)
    {
        User user = testconn.Find<User>(userName);
        user.energy = energy;
        user.gold = gold;
        user.moonrock = moonrock;
        user.playTime = playTime;

        testconn.Update(user);
    }

    public void Update_DesginShopLevel(int level)
    {
        User user = testconn.Find<User>(userName);
        user.designshopLevel = level;
        testconn.Update(user);
    }

    public void Update_ItemShopLevel(int level)
    {
        User user = testconn.Find<User>(userName);
        user.itemshopLevel = level;
        testconn.Update(user);
    }

    public void Update_LoomLevel(int level)
    {
        User user = testconn.Find<User>(userName);
        user.loomLevel = level;
        testconn.Update(user);
    }

    public void Update_FillerLevel(int level)
    {
        User user = testconn.Find<User>(userName);
        user.fillerLevel = level;
        testconn.Update(user);
    }

    public void Update_DecoLevel(int level)
    {
        User user = testconn.Find<User>(userName);
        user.decoLevel = level;
        testconn.Update(user);
    }

    public void Update_IsOpen(bool isOpen)
    {
        User user = testconn.Find<User>(userName);
        user.isOpen = isOpen;
        testconn.Update(user);
    }

    public bool isIn_Inventory(string itemName)
    {
        return testconn.Table<Inventory>()
            .Any(x => x.itemName == itemName);
    }

    public void Insert_InventoryItem(string itemName, ItemType itemType, int count)
    {
        Inventory inven = new Inventory();
        inven.itemName = itemName;
        inven.itemType = itemType;
        inven.count = count;

        testconn.Insert(inven);
    }

    public void Change_InventoryItem_Count(string itemName, int delta)
    {
        Inventory inven = testconn.Find<Inventory>(itemName);
        inven.count += delta;
        testconn.Update(inven);
    }

    public bool Have_Design(string blanketName)
    {
        return testconn.Table<Design>()
            .Any(x => x.blanketName == blanketName);
    }

    public void Insert_Design(string blanketName)
    {
        Design design = new Design();
        design.blanketName = blanketName;

        testconn.Insert(design);
    }

    public void Insert_InteriorItem(string interiorName, InteriorType interiorType, int count)
    {
        for(int i = 0; i < count; i++)
        {
            Interior interior = new Interior();
            interior.interiorName = interiorName;
            interior.interiorType = interiorType;
            interior.isSet = false;

            testconn.Insert(interior);
        }
    }

    public bool Have_Tile(string tileName)
    {
        return testconn.Table<Interior>()
            .Any(x => x.interiorName == tileName);
    }

    public void Insert_Tile(string tileName, InteriorType interiorType)
    {
        Interior interior = new Interior();
        interior.interiorName = tileName;
        interior.interiorType = interiorType;
        interior.isSet = false;

        testconn.Insert(interior);
    }
}
