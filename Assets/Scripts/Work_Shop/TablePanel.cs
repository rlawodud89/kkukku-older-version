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

    void Awake()
    {
        BlanketBtnDic = new Dictionary<string, BlanketBtn>();
        gameManager = GameManager.getInstance();
        gameManager.OnTableBlanketChanged += TableBlanketChanged;
    }

    void OnDestroy()
    {
        gameManager.OnTableBlanketChanged -= TableBlanketChanged;
    }

    void Start()
    {
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
        if (SelectedBtn == null) return;

        string Selected_itemName = SelectedBtn.blanketScript.itemName;
        int Selected_BlanketCount = SelectedBtn.BlanketCount;

        if (gameManager.Use_Table_Blanket(tableID, Selected_itemName, Selected_BlanketCount))
        {
            gameManager.Add_InventoryItem(Selected_itemName, Selected_BlanketCount);
        }

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
        if (BlanketBtnDic.TryGetValue(blanketName, out var btn))
        {
            if (SelectedBtn == btn) SelectedBtn = null;
        }

        BlanketBtnDic.Remove(blanketName);
        if (BlanketBtnDic.Count == 0) OnFullChanged?.Invoke(false);
    }

    private void TableBlanketChanged(int tableID, string blanketName, int delta)
    {
        if (this.tableID != tableID || delta == 0) return;

        if (delta > 0) // 이불 추가됐다면
        {
            Add_BlanketBtn(blanketName, delta);
        }
        else // 이불 삭제되었다면
        {
            if (BlanketBtnDic.TryGetValue(blanketName, out var btn) && btn != null)
            {
                btn.Change_BlanketCount(delta);
            }
        }
    }

}
