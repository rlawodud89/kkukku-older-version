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
    private static SQLiteConnection conn = new SQLiteConnection(testdbPath);
    private static string userName = "user";

    //싱글톤 패턴 위한 private 생성자, 인스턴스 반환 정적 메서드
    private DBManager() { }
    public static DBManager getInstance() { return instance; }

    public void InitDB()
    {
        conn.CreateTable<User>();
        conn.CreateTable<Inventory>();
        conn.CreateTable<Design>();
        conn.CreateTable<WorkShop>();
        conn.CreateTable<ShopTable>();
        conn.CreateTable<WorkRoom>();
        conn.CreateTable<Interior>();
        conn.CreateTable<Tile>();
        conn.CreateTable<QuestBox>();
        conn.CreateTable<LetterBox>();

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

        conn.Insert(user);

        // TODO: 처음에 기본으로 주는 아이템 저장
    }

    public User Get_User()
    {
        return conn.Find<User>(userName); //지정한 이름(기본키)으로 찾기
    }

    public void Update_User(int energy, int gold, int moonrock, float playTime)
    {
        User user = conn.Find<User>(userName);
        user.energy = energy;
        user.gold = gold;
        user.moonrock = moonrock;
        user.playTime = playTime;

        conn.Update(user);
    }

    public void Update_DesginShopLevel(int level)
    {
        User user = conn.Find<User>(userName);
        user.designshopLevel = level;
        conn.Update(user);
    }

    public void Update_ItemShopLevel(int level)
    {
        User user = conn.Find<User>(userName);
        user.itemshopLevel = level;
        conn.Update(user);
    }

    public void Update_LoomLevel(int level)
    {
        User user = conn.Find<User>(userName);
        user.loomLevel = level;
        conn.Update(user);
    }

    public void Update_FillerLevel(int level)
    {
        User user = conn.Find<User>(userName);
        user.fillerLevel = level;
        conn.Update(user);
    }

    public void Update_DecoLevel(int level)
    {
        User user = conn.Find<User>(userName);
        user.decoLevel = level;
        conn.Update(user);
    }

    public void Update_IsOpen(bool isOpen)
    {
        User user = conn.Find<User>(userName);
        user.isOpen = isOpen;
        conn.Update(user);
    }

    public bool Have_Inventory(string itemName)
    {
        return conn.Table<Inventory>()
            .Any(x => x.itemName == itemName);
    }

    public void Insert_InventoryItem(string itemName, ItemType itemType, int count)
    {
        Inventory inven = new Inventory();
        inven.itemName = itemName;
        inven.itemType = itemType;
        inven.count = count;

        conn.Insert(inven);
    }

    public bool Change_InventoryItem_Count(string itemName, int delta)
    {
        Inventory inven = conn.Find<Inventory>(itemName);

        if (inven.count + delta < 0) return false;

        else if (inven.count + delta == 0)
        {
            conn.Delete(inven);
        }
        else
        {
            inven.count += delta;
            conn.Update(inven);
        }
        return true;
    }

    public bool Have_Design(string blanketName)
    {
        return conn.Table<Design>()
            .Any(x => x.blanketName == blanketName);
    }

    public void Insert_Design(string blanketName)
    {
        Design design = new Design();
        design.blanketName = blanketName;

        conn.Insert(design);
    }

    public bool Have_InteriorItem(string interirorName)
    {
        return conn.Table<Interior>()
            .Any(x => x.interiorName == interirorName);
    }

    public void Insert_InteriorItem(string interiorName, InteriorType interiorType, int count) // 완전 새로운 인테리어 아이템 추가
    {
        for (int i = 0; i < count; i++)
        {
            Interior interior = new Interior();
            interior.interiorName = interiorName;
            interior.interiorType = interiorType;
            interior.isSet = false;

            conn.Insert(interior);
        }
    }

    public void Insert_Tile(string tileName, InteriorType interiorType)
    {
        Interior interior = new Interior();
        interior.interiorName = tileName;
        interior.interiorType = interiorType;
        interior.isSet = false;

        conn.Insert(interior);
    }

    public List<Inventory> Select_Yarn()
    {
        return conn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.YARN)
               .ToList();
    }

    public List<Inventory> Select_Cotton()
    {
        return conn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.COTTON)
               .ToList();
    }


    public List<Inventory> Select_Material()
    {
        return conn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.MATERIAL)
               .ToList();
    }

    public List<Inventory> Select_Blanket()
    {
        return conn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.BLANKET)
               .ToList();
    }

    public List<Inventory> Select_Snack()
    {
        return conn.Table<Inventory>()
               .Where(x => x.itemType == ItemType.SNACK)
               .ToList();
    }

    public List<(string itemName, int count)> Select_RoomInterior()
    {
        return conn.Table<Interior>()
                .Where(x => x.isSet == false
                && (x.interiorType == InteriorType.ROOM_INTERIROR || x.interiorType == InteriorType.WORKER))
                .GroupBy(x => x.interiorName)
                .Select(g => (g.Key, g.Count())) // Key: GroupBy에서 사용한 키 (interiorName), Count(): 해당하는 키 그룹의 튜플 개수
                .ToList();
    }

    public List<ShopTable> Select_Table_Blanket(int tableID)
    {
        return conn.Table<ShopTable>()
            .Where(x => x.tableID == tableID)
            .ToList();
    }

    public bool Have_Table_Blanket(int tableID, string blanketName)
    {
        return conn.Table<ShopTable>()
            .Any(x => x.tableID == tableID && x.blanketName == blanketName);
    }

    public void Insert_TableBlanket(int tableID, string blanketName, int count)
    {
        try
        {
            ShopTable sh = new ShopTable();
            sh.tableID = tableID;
            sh.blanketName = blanketName;
            sh.count = count;

            conn.Insert(sh);
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: ShopTable PK 위반)");
        }

    }

    public bool Change_TableBlanket_Count(int tableID, string blanketName, int delta)
    {
        try
        {
            ShopTable sh = conn.Table<ShopTable>()
             .Where(x => x.tableID == tableID && x.blanketName == blanketName)
             .FirstOrDefault();
            int current_count = sh.count;

            if (current_count + delta < 0) return false;

            else if (current_count + delta == 0)
            {
                conn.Execute("DELETE FROM ShopTable WHERE tableID = ? AND blanketName = ?",
                    tableID, blanketName);
            }
            else
            {
                current_count += delta;
                conn.Execute("UPDATE ShopTable SET count = ? WHERE tableID = ? AND blanketName = ?",
                    current_count, tableID, blanketName);
            }
            return true;
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: ShopTable PK 위반)");
            return false;
        }
        
    }

    public WorkShop Select_WorkShop(int tableID)
    {
        return conn.Find<WorkShop>(tableID);
    }

    public bool Any_Table_Blanket(int tableID)
    {
        return conn.Table<ShopTable>()
            .Any(x => x.tableID == tableID);
    }

    public bool Set_InteriorItem(string interiorName, int x, int y) // 없던 인테리어 아이템을 좌표에 위치시키는 메서드
    {
        try
        {
            // 아직 설치하지 않은 interiorName의 아이템 중 가장 오래된 것 하나를 선택해서 update
            int affectedRows = conn.Execute("UPDATE Interior SET isSet = 1, x = ?, y = ? " +
               "WHERE rowid = (SELECT rowid FROM Interior WHERE interiorName = ? AND isSet = 0 " +
                               "ORDER BY rowid ASC LIMIT 1",
                               x, y, interiorName);

            return affectedRows > 0; // update된 행이 있다면 true, 없다면 false
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }

    public bool Change_InteriorItem_Pos(int beforeX, int beforeY, int afterX, int afterY) // 인테리어 아이템 위치 변경
    {
        try
        {
            int affectedRows = conn.Execute("UPDATE Interior SET x = ?, y = ? WHERE isSet = 1 AND x = ? AND y = ?",
            afterX, afterY, beforeX, beforeY);

            return affectedRows > 0;
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }

    public bool NotSet_InteriorItem(int x, int y) // 좌표에 위치되어 있던 인테리어 아이템 빼는 메서드
    {
        try
        {
            int affectedRows = conn.Execute("UPDATE Interior SET isSet = 0 WHERE isSet = 1 AND x = ? AND y = ?",
                    x, y);

            return affectedRows > 0;
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }



}
