using System;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.Linq;
using static UnityEditor.Progress;


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

    public bool Have_Inventory(string itemName)
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

    public bool Change_InventoryItem_Count(string itemName, int delta)
    {
        Inventory inven = testconn.Find<Inventory>(itemName);

        if (inven.count + delta < 0) return false;

        else if (inven.count + delta == 0)
        {
            testconn.Delete(inven);
        }
        else
        {
            inven.count += delta;
            testconn.Update(inven);
        }
        return true;
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

    public bool Have_InteriorItem(string interirorName)
    {
        return testconn.Table<Interior>()
            .Any(x => x.interiorName == interirorName);
    }

    public void Insert_InteriorItem(string interiorName, InteriorType interiorType, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Interior interior = new Interior();
            interior.interiorName = interiorName;
            interior.interiorType = interiorType;
            interior.isSet = false;

            testconn.Insert(interior);
        }
    }

    public bool Set_InteriorItem(string interiorName, int x, int y)
    {
        Interior inte = testconn.Table<Interior>()
                 .Where(x => x.interiorName == interiorName && x.isSet == false)
                 .FirstOrDefault();

        if (inte == null) return false;
        else
        {
            inte.isSet = true;
            inte.x = x;
            inte.y = y;
            testconn.Update(inte);
            return true;
        }
    }

    public void Insert_Tile(string tileName, InteriorType interiorType)
    {
        Interior interior = new Interior();
        interior.interiorName = tileName;
        interior.interiorType = interiorType;
        interior.isSet = false;

        testconn.Insert(interior);
    }

    public List<Inventory> Select_Material()
    {
        return testconn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.MATERIAL)
               .ToList();

    }

    public List<Inventory> Select_Blanket()
    {
        return testconn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.BLANKET)
               .ToList();
    }

    public List<Inventory> Select_Snack()
    {
        return testconn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.SNACK)
               .ToList();
    }

    public List<(string itemName, int count)> Select_RoomInterior()
    {
        return testconn.Table<Interior>()
                .Where(x => x.isSet == false)
                .GroupBy(x => x.interiorName)
                .Select(g => (g.Key, g.Count())) // Key: GroupBy에서 사용한 키 (interiorName), Count(): 해당하는 키 그룹의 튜플 개수
                .ToList();
    }
}
