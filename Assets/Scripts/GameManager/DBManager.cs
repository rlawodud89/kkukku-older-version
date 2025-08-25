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

    public bool HaveDB()
    {
        var tableInfo = conn.GetTableInfo("User");
        bool exists = tableInfo.Count > 0;
        return exists;
    }

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
        conn.CreateTable<StoreItem>();

        // 사용자 정보
        User user = new User();
        user.name = "user";
        user.energy = 0;
        user.gold = 1000;
        user.moonrock = 1000;
        user.todayEnergy = 0;
        user.todayGold = 0;
        user.todayMoonrock = 0;
        user.playTime = 25200; // Day1 7시
        user.designshopLevel = 1;
        user.itemshopLevel = 1;
        user.loomLevel = 1;
        user.fillerLevel = 1;
        user.decoLevel = 1;
        user.endScene = "Work_Shop";
        user.isOpen = false;
        user.bgSound = 0.5f;
        user.effectSound = 0.5f;
        conn.Insert(user);

        // 재료
        Insert_InventoryItem("꿈실", ItemType.MATERIAL, 6);
        Insert_InventoryItem("운무솜", ItemType.MATERIAL, 6);
        Insert_InventoryItem("달조각", ItemType.MATERIAL, 6);

        // 이불 디자인
        Insert_Design("기본이불");

        // 타일
        Insert_New_Tile("나무벽", InteriorType.WALL_TILE);
        Insert_New_Tile("나무바닥", InteriorType.FLOOR_TILE);

        Tile tile = new Tile();
        tile.tilePos = TilePosType.ROOM_FLOOR;
        tile.tileName = "나무바닥";
        conn.Insert(tile);
        tile.tilePos = TilePosType.ROOM_WALL;
        tile.tileName = "나무벽";
        conn.Insert(tile);
        tile.tilePos = TilePosType.SHOP_FLOOR;
        tile.tileName = "나무바닥";
        conn.Insert(tile);
        tile.tilePos = TilePosType.SHOP_WALL;
        tile.tileName = "나무벽";
        conn.Insert(tile);

        // 가게 인테리어
        Insert_InteriorItem("나무벽장", InteriorType.SHOP_INTERIOR, 1);
        Insert_InteriorItem("나무진열장", InteriorType.SHOP_INTERIOR, 1);

        WorkShop workShop = new WorkShop();
        workShop.tableID = 1;
        workShop.tableName = "나무벽장";
        conn.Insert(workShop);
        workShop.tableID = 2;
        workShop.tableName = "나무벽장";
        conn.Insert(workShop);
        workShop.tableID = 3;
        workShop.tableName = "나무진열장";
        conn.Insert(workShop);
        workShop.tableID = 4;
        workShop.tableName = "나무진열장";
        conn.Insert(workShop);

        // 작업실 인테리어
        Insert_InteriorItem("이불보관함", InteriorType.ROOM_INTERIROR, 1);
        Insert_InteriorItem("재료보관함", InteriorType.ROOM_INTERIROR, 1);
        Insert_InteriorItem("간식박스", InteriorType.ROOM_INTERIROR, 1);
        Insert_InteriorItem("특별제작대", InteriorType.ROOM_INTERIROR, 1);

        Set_InteriorItem("이불보관함", -2.925f, 2.9166f);
        Set_InteriorItem("재료보관함", -5.85f, 0.9722f);
        Set_InteriorItem("간식박스", -7.8f, 0.4861f);
        Set_InteriorItem("특별제작대", -5.3625f, -1.21525f);

        // 작업실 직원
        Insert_InteriorItem("여우직원", InteriorType.WORKER, 1);
        Insert_InteriorItem("양직원", InteriorType.WORKER, 1);
        Insert_InteriorItem("고양이직원", InteriorType.WORKER, 1);

        Set_InteriorItem("여우직원", -0.3130001f, -1.70135f);
        Set_InteriorItem("양직원", 1.1495f, -0.9722f);
        Set_InteriorItem("고양이직원", 2.612f, -0.24305f);

        Insert_Worker("여우직원", -0.3130001f, -1.70135f);
        Insert_Worker("양직원", 1.1495f, -0.9722f);
        Insert_Worker("고양이직원", 2.612f, -0.24305f);

        // 상점 아이템
        Insert_StoreItem(StoreType.SHOP_INTERIOR, "짙은나무벽장");
        Insert_StoreItem(StoreType.SHOP_INTERIOR, "흰색벽장");
        Insert_StoreItem(StoreType.SHOP_INTERIOR, "분홍색진열장");

        Insert_StoreItem(StoreType.ROOM_INTERIROR, "나무의자");
        Insert_StoreItem(StoreType.ROOM_INTERIROR, "간식박스");
        Insert_StoreItem(StoreType.ROOM_INTERIROR, "아이보리소파");

        Insert_StoreItem(StoreType.TILE, "통나무벽");
        Insert_StoreItem(StoreType.TILE, "돌벽");
        Insert_StoreItem(StoreType.TILE, "흰나무바닥");

        Insert_StoreItem(StoreType.BLANKET, "빨강이불");
        Insert_StoreItem(StoreType.BLANKET, "우주이불");
    }

    public User Get_User()
    {
        return conn.Find<User>(userName); //지정한 이름(기본키)으로 찾기
    }

    public void Update_PlayTime(float playTime)
    {
        User user = conn.Find<User>(userName);
        user.playTime = playTime;
        conn.Update(user);
    }

    public void Update_Energy(int energy)
    {
        User user = conn.Find<User>(userName);
        user.energy = energy;
        conn.Update(user);
    }

    public void Update_Gold(int gold)
    {
        User user = conn.Find<User>(userName);
        user.gold = gold;
        conn.Update(user);
    }

    public void Update_Moonrock(int moonrock)
    {
        User user = conn.Find<User>(userName);
        user.moonrock = moonrock;
        conn.Update(user);
    }

    public void Update_TodayEnergy(int todayEnergy)
    {
        User user = conn.Find<User>(userName);
        user.todayEnergy = todayEnergy;
        conn.Update(user);
    }


    public void Update_TodayGold(int todayGold)
    {
        User user = conn.Find<User>(userName);
        user.todayGold = todayGold;
        conn.Update(user);
    }

    public void Update_TodayMoonrock(int todayMoonrock)
    {
        User user = conn.Find<User>(userName);
        user.todayMoonrock = todayMoonrock;
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

    public void Update_BgSound(float bgSound)
    {
        User user = conn.Find<User>(userName);
        user.bgSound = bgSound;
        conn.Update(user);
    }

    public void Update_EffectSound(float effectSound)
    {
        User user = conn.Find<User>(userName);
        user.effectSound = effectSound;
        conn.Update(user);
    }

    public void Update_EndScene(string endScene)
    {
        User user = conn.Find<User>(userName);
        user.endScene = endScene;
        conn.Update(user);
    }

    public void Reset_User_Todays()
    {
        User user = conn.Find<User>(userName);
        user.todayEnergy = 0;
        user.todayGold = 0;
        user.todayMoonrock = 0;
        conn.Update(user);
    }

    public bool Have_Inventory(string itemName)
    {
        return conn.Table<Inventory>()
            .Any(i => i.itemName == itemName);
    }

    public Inventory Select_InventoryItem(string itemName)
    {
        return conn.Table<Inventory>()
            .Where(i => i.itemName == itemName)
            .FirstOrDefault();
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

    public List<Design> Select_Design()
    {
        return conn.Table<Design>()
            .ToList();
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

    public void Insert_New_Tile(string tileName, InteriorType interiorType)
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

    public List<(string itemName, int count)> Select_RoomInterior_Inventory()
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

    public bool Insert_Worker(string workerName, float x, float y)
    {
        try
        {
            WorkRoom newWorkRoom = new WorkRoom();
            conn.Insert(newWorkRoom);

            int affectedRows = conn.Execute("UPDATE Interior SET ID = ? " +
                "WHERE interiorName = ? AND isSet = 1 AND x = ? AND y = ?",
                newWorkRoom.workerID, workerName, x, y);

            return affectedRows > 0;

        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }

    public bool Delete_Worker(int workerId)
    {
        try
        {
            WorkRoom workRoom = conn.Find<WorkRoom>(workerId);
            conn.Delete(workRoom);
            return true;
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }

    public bool Set_InteriorItem(string interiorName, float x, float y) // 없던 인테리어 아이템을 좌표에 위치시키는 메서드
    {
        try
        {
            // 아직 설치하지 않은 interiorName의 아이템 중 가장 오래된 것 하나를 선택해서 update
            int affectedRows = conn.Execute("UPDATE Interior SET isSet = 1, x = ?, y = ? " +
               "WHERE rowid = (SELECT rowid FROM Interior WHERE interiorName = ? AND isSet = 0 " +
                               "ORDER BY rowid ASC LIMIT 1)",
                               x, y, interiorName);

            return affectedRows > 0; // update된 행이 있다면 true, 없다면 false
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }

    public bool Change_InteriorItem_Pos(float beforeX, float beforeY, float afterX, float afterY) // 인테리어 아이템 위치 변경
    {
        try
        {
            int affectedRows = conn.Execute("UPDATE Interior SET x = ?, y = ? " +
                "WHERE isSet = 1 AND ABS(x - ?) < 0.01 AND ABS(y - ?) < 0.01",
            afterX, afterY, beforeX, beforeY);

            return affectedRows > 0;
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }

    public bool NotSet_InteriorItem(float x, float y) // 좌표에 위치되어 있던 인테리어 아이템 빼는 메서드
    {
        try
        {
            Interior interior = conn.Query<Interior>(
                "SELECT * FROM Interior WHERE isSet = 1 AND ABS(x - ?) < 0.01 AND ABS(y - ?) < 0.01",
                    x, y)
                .FirstOrDefault();

            if (interior.interiorType == InteriorType.WORKER)
            {
                if (!Delete_Worker(interior.ID)) return false;
            }

            int affectedRows = conn.Execute("UPDATE Interior SET isSet = 0 WHERE isSet = 1 AND ABS(x - ?) < 0.01 AND ABS(y - ?) < 0.01",
                    x, y);

            return affectedRows > 0;
        }
        catch (SQLiteException)
        {
            Debug.LogError("쿼리 실패 (예상: Interior 제약 위반)");
            return false;
        }
    }

    public List<Interior> Select_Current_RoomInterior()
    {
        return conn.Table<Interior>()
            .Where(i => i.isSet == true && (i.interiorType == InteriorType.ROOM_INTERIROR || i.interiorType == InteriorType.WORKER))
            .ToList();
    }

    public Tile Select_Tile(TilePosType tilePosType)
    {
        return conn.Find<Tile>(tilePosType);
    }

    public void Update_Tile(TilePosType tilePosType, string tileName)
    {
        Tile tile = conn.Find<Tile>(tilePosType);
        tile.tileName = tileName;
        conn.Update(tile);
    }

    public List<Interior> Select_FloorTile_Inventory()
    {
        return conn.Table<Interior>()
            .Where(i => i.interiorType == InteriorType.FLOOR_TILE)
            .ToList();
    }

    public List<Interior> Select_WallTile_Inventory()
    {
        return conn.Table<Interior>()
            .Where(i => i.interiorType == InteriorType.WALL_TILE)
            .ToList();
    }

    public void Update_ShopTableInterior(int tableID, string interiorName)
    {
        WorkShop table = conn.Find<WorkShop>(tableID);
        table.tableName = interiorName;
        conn.Update(table);
    }

    public List<Interior> Select_ShopInterior_Inventory()
    {
        return conn.Table<Interior>()
            .Where(i => i.interiorType == InteriorType.SHOP_INTERIOR)
            .ToList();
    }

    public List<QuestBox> Select_All_Quest()
    {
        return conn.Table<QuestBox>()
            .ToList();
    }

    public void Insert_Quest(string questName)
    {
        QuestBox quest = new QuestBox();
        quest.questName = questName;
        conn.Insert(quest);
    }

    public void Delete_Quest(string questName)
    {
        conn.Delete<QuestBox>(questName);
    }

    public void Update_Quest_Process(string questName, int process)
    {
        QuestBox quest = conn.Find<QuestBox>(questName);
        quest.process = process;
        conn.Update(quest);
    }

    public void Update_Quest_IsCompleted(string questName, bool isCompleted)
    {
        QuestBox quest = conn.Find<QuestBox>(questName);
        quest.isCompleted = isCompleted;
        conn.Update(quest);
    }

    public void Update_Quest_GetReward(string questName, bool getReward)
    {
        QuestBox quest = conn.Find<QuestBox>(questName);
        quest.getReward = getReward;
        conn.Update(quest);
    }

    public int Select_Worker_ID(float x, float y)
    {
        Interior worker = conn.Query<Interior>(
                "SELECT * FROM Interior WHERE isSet = 1 AND ABS(x - ?) < 0.01 AND ABS(y - ?) < 0.01",
                    x, y)
                .FirstOrDefault();

        return worker.ID;
    }

    public WorkRoom Select_Worker_Info(int workerID)
    {
        return conn.Find<WorkRoom>(workerID);
    }

    public void Change_Worker_Stamina(int workerID, int delta)
    {
        WorkRoom worker = conn.Find<WorkRoom>(workerID);
        worker.stamina += delta;
        conn.Update(worker);
    }

    public void Update_Worker_workingItem(int workerID, string workingItem)
    {
        WorkRoom worker = conn.Find<WorkRoom>(workerID);
        worker.workItem = workingItem;
        conn.Update(worker);
    }

    public void Update_Worker_WorkingPercent(int workerID, float workingPercent)
    {
        WorkRoom worker = conn.Find<WorkRoom>(workerID);
        worker.workingPercent = workingPercent;
        conn.Update(worker);
    }

    public List<LetterBox> Select_Current_Letter()
    {
        return conn.Table<LetterBox>()
            .ToList();
    }

    public void Insert_Letter(string letterName)
    {
        LetterBox letter = new LetterBox();
        letter.letterName = letterName;
        conn.Insert(letter);
    }

    public void Delete_Letter(string letterName)
    {
        LetterBox letter = conn.Find<LetterBox>(letterName);
        conn.Delete(letter);
    }

    public List<StoreItem> Select_StoreItem(StoreType storeType)
    {
        return conn.Table<StoreItem>()
            .Where(i => i.storeType == storeType)
            .ToList();
    }

    public void Insert_StoreItem(StoreType storeType, string itemName)
    {
        StoreItem storeItem = new StoreItem();
        storeItem.storeType = storeType;
        storeItem.itemName = itemName;

        conn.Insert(storeItem);
    }

    public void Delete_All_StoreItem()
    {
        conn.Execute("DELETE FROM StoreItem");
    }

    public bool Have_StoreItem(string itemName)
    {
        return conn.Table<StoreItem>()
           .Any(i => i.itemName == itemName);
    }

}
