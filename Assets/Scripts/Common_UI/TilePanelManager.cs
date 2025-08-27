using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TilePanelManager : MonoBehaviour
{
    public GameObject tilePanel;
    public Transform FloorContent;
    public Transform WallContent;
    public GameObject TileButton;

    private GameManager gameManager;
    private List<InteriorScript> floorTiles;
    private List<InteriorScript> wallTiles;

    private QuestManager questManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
        floorTiles = gameManager.Get_FloorTile_Inventory();
        wallTiles = gameManager.Get_WallTile_Inventory();

        InitScroll();
        ClickFloorChooseBtn();

        questManager= QuestManager.Instance;
    }

    private void InitScroll()
    {
        foreach (InteriorScript tile in floorTiles)
        {
            GameObject newButton = Instantiate(TileButton, FloorContent);

            TileButton newFloorTile = newButton.GetComponent<TileButton>();
            newFloorTile.tileImage.sprite = tile.image;
            newFloorTile.button.onClick.AddListener(() => ClickFloorTileBtn(tile));
        }

        foreach (InteriorScript tile in wallTiles)
        {
            GameObject newButton = Instantiate(TileButton, WallContent);

            TileButton newWallTile = newButton.GetComponent<TileButton>();
            newWallTile.tileImage.sprite = tile.image;
            newWallTile.button.onClick.AddListener(() => ClickWallTileBtn(tile));
        }
    }

    public void ClickExitBtn()
    {
        tilePanel.gameObject.SetActive(false);
    }

    public void ClickFloorChooseBtn()
    {
        FloorContent.gameObject.SetActive(true);
        WallContent.gameObject.SetActive(false);
    }

    public void ClickWallChooseBtn()
    {
        FloorContent.gameObject.SetActive(false);
        WallContent.gameObject.SetActive(true);
    }

    public void ClickFloorTileBtn(InteriorScript floorTile)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;  // 현재 씬
        TilePosType tilePos;
        if (currentSceneName == "Work_Shop") {
            tilePos = TilePosType.SHOP_FLOOR;

            // 퀘스트
            AddQuestProcess.Instance.AddProcessToQuest("인테리어 바꾸기");

        }
        else if (currentSceneName == "Work_Room") tilePos = TilePosType.ROOM_FLOOR;
        else return;

        gameManager.Set_Current_Tile(tilePos, floorTile.interiorName);
    }

    public void ClickWallTileBtn(InteriorScript wallTile)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;  // 현재 씬
        TilePosType tilePos;
        if (currentSceneName == "Work_Shop") tilePos = TilePosType.SHOP_WALL;
        else if (currentSceneName == "Work_Room") tilePos = TilePosType.ROOM_WALL;
        else return;

        gameManager.Set_Current_Tile(tilePos, wallTile.interiorName);
    }


}
