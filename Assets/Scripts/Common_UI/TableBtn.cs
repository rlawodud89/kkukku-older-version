using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBtn : MonoBehaviour
{
    public int tableID;
    public InteriorScript interiorScript;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();    
    }

    public void ClickTableBtn()
    {
        Debug.LogWarning(tableID + interiorScript.interiorName);
        gameManager.Set_ShopTableInterior(tableID, interiorScript.interiorName);

        if(interiorScript.tableType == TableType.WALL_TABLE)
        {
            GameObject WallTable1Btn = GameObject.Find("WallTable1Btn");
            GameObject WallTable2Btn = GameObject.Find("WallTable2Btn");
            WallTable1Btn.SetActive(false);
            WallTable2Btn.SetActive(false);
        }
        else
        {
            GameObject Table1Btn = GameObject.Find("Table1Btn");
            GameObject Table2Btn = GameObject.Find("Table2Btn");

            Table1Btn.SetActive(false);
            Table2Btn.SetActive(false);
        }
        
    }
}
