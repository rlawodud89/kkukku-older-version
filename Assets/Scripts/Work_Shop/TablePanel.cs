using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TablePanel : MonoBehaviour
{
    public TableAddPanel AddPanel;

    public Transform ScrollContent;
    public GameObject BlanketBtn;
    public int tableID;
    public event Action<bool> OnFullChanged;

    private BlanketBtn SelectedBtn;
    private Dictionary<string, BlanketBtn> BlanketBtnDic;
    private GameManager gameManager;


    void Start()
    {
        BlanketBtnDic = new Dictionary<string, BlanketBtn>();
        gameManager = GameManager.getInstance();

        AddPanel.gameObject.SetActive(false);

        InitScroll();
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void ClickAddBtn()
    {
        AddPanel.gameObject.SetActive(true);
    }


    public void ClickDeleteBtn()
    {
        string Selected_itemName = SelectedBtn.blanketScript.itemName;
        int Selected_BlanketCount = SelectedBtn.BlanketCount;

        gameManager.Add_InventoryItem(Selected_itemName, Selected_BlanketCount); // 이불장에서 뺀 만큼 다시 인벤토리로 이동
        AddPanel.Add_BlanketAddBtn(Selected_itemName, Selected_BlanketCount);

        Delete_In_BlanketBtnDic(Selected_itemName); // 이불장에서 삭제
        Destroy(SelectedBtn.gameObject);
        SelectedBtn = null;
    }

    private void InitScroll()
    {
        List<(ItemScript blanket, int count)> Blankets = new List<(ItemScript blanket, int count)>();

        Blankets = gameManager.Get_Table_Blanket(tableID);

        foreach (var bk in Blankets)
        {
            GameObject newButton = Instantiate(BlanketBtn, ScrollContent);
            BlanketBtn newBlanketBtn = newButton.GetComponent<BlanketBtn>();
            newBlanketBtn.TablePanel = this;
            newBlanketBtn.blanketScript = bk.blanket;
            newBlanketBtn.BlanketCount = bk.count;

            BlanketBtnDic.Add(bk.blanket.itemName, newBlanketBtn);
        }
    }

    public void Chanage_SelectedBtn(BlanketBtn AfterBtn)
    {
        if (AfterBtn == null)
        {
            SelectedBtn.Set_NotSelected();
            SelectedBtn = null;
            return;
        }

        if (SelectedBtn != null)
        {
            BlanketBtn BeforeBtn = SelectedBtn;
            BeforeBtn.Set_NotSelected();
        }
        SelectedBtn = AfterBtn;
        SelectedBtn.Set_Selected();
    }

    
    public void Add_BlanketBtn(string blanketName, int count)
    {
        if (count <= 0) return;

        if (BlanketBtnDic.ContainsKey(blanketName))
        {
            BlanketBtnDic[blanketName].Change_BlanketCount(count);
        }
        else
        {
            GameObject newButton = Instantiate(BlanketBtn, ScrollContent);

            BlanketBtn newBlanketBtn = newButton.GetComponent<BlanketBtn>();
            newBlanketBtn.TablePanel = this;
            newBlanketBtn.blanketScript = gameManager.Get_Blanket(blanketName);
            newBlanketBtn.Set_BlanketCount(count);

            BlanketBtnDic.Add(blanketName, newBlanketBtn);
            if (BlanketBtnDic.Count == 1) OnFullChanged?.Invoke(true); 
        }
    }

    public void Delete_In_BlanketBtnDic(string blanketName)
    {
        BlanketBtnDic.Remove(blanketName);
        if(BlanketBtnDic.Count == 0) OnFullChanged?.Invoke(false);
    }

}
