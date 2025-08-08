using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableAddPanel : MonoBehaviour
{
    public TablePanel TablePanel;

    public Transform ScrollContent;
    public GameObject BlanketBtn;
    public Image SelectImg;
    public TMP_InputField CountInput;

    private BlanketAddBtn SelectedBtn;
    private Dictionary<string, BlanketAddBtn> BlanketAddBtnDic = new Dictionary<string, BlanketAddBtn>();
    private GameManager gameManager;

    void Awake()
    {
        BlanketAddBtnDic = new Dictionary<string, BlanketAddBtn>();
        gameManager = GameManager.getInstance();
        gameManager.OnBlanketInvenChanged += BlanketInvenChanged;

        InitScroll();
    }

    public void ClickAddXBtn()
    {
        gameObject.SetActive(false);
    }


    public void ClickPlusBtn()
    {
        if (CountInput.text == null || SelectedBtn == null) return;
    
        int input_count = int.Parse(CountInput.text);
        if(input_count > 0)
        {
            string Selected_itemName = SelectedBtn.blanketScript.itemName;

            /*if (SelectedBtn.Change_BlanketCount(-input_count) 
                && gameManager.Use_InventoryItem(Selected_itemName, input_count))*/
            if (gameManager.Use_InventoryItem(Selected_itemName, input_count))
            {
                TablePanel.Add_BlanketBtn(Selected_itemName, input_count);
                gameManager.Add_Table_Blanket(TablePanel.tableID, Selected_itemName, input_count);
            }
        }
        else 
        {
            Debug.Log("0 이하 Input");
        }
        
        if(SelectedBtn != null) SelectedBtn.Set_NotSelected();
        SelectedBtn = null;
        Color color = SelectImg.color;
        color.a = 0f;
        SelectImg.color = color;
        CountInput.text = null;
    }

    private void InitScroll()
    {
        List<(ItemScript blanket, int count)> Blankets = gameManager.Get_Blanket_Inventory();

        foreach(var bk in Blankets)
        {
            GameObject newButton = Instantiate(BlanketBtn, ScrollContent);
            BlanketAddBtn newBlanketAddBtn = newButton.GetComponent<BlanketAddBtn>();
            newBlanketAddBtn.AddPanel = this;
            newBlanketAddBtn.blanketScript = bk.blanket;
            newBlanketAddBtn.BlanketCount = bk.count;

            BlanketAddBtnDic.Add(bk.blanket.itemName, newBlanketAddBtn);
        }
    }

    public void Chanage_SelectedBtn(BlanketAddBtn AfterBtn)
    {
        Color color = SelectImg.color; //하단에 뜨는 이미지 조절 위해

        if (AfterBtn == null)
        {
            SelectedBtn.Set_NotSelected();
            SelectedBtn = null;
            //선택된 이미지 없으므로 안보이게 만들기
            color.a = 0f; //투명하게
            SelectImg.color = color;

            return;
        }

        if (SelectedBtn != null)
        {
            BlanketAddBtn BeforeBtn = SelectedBtn;
            BeforeBtn.Set_NotSelected();
        }
        AfterBtn.Set_Selected();
        SelectedBtn = AfterBtn;
        SelectImg.sprite = AfterBtn.blanketImage.sprite; //이미지 교체
        //선택된 이미지 보이게 만들기
        color.a = 1f; //불투명하게
        SelectImg.color = color;
    }

    public void Add_BlanketAddBtn(string blanketName, int count)
    {
        if (count <= 0) return;

        if (BlanketAddBtnDic.ContainsKey(blanketName))
        {
            BlanketAddBtnDic[blanketName].Change_BlanketCount(count);
        }
        else
        {
            GameObject newButton = Instantiate(BlanketBtn, ScrollContent);

            BlanketAddBtn newBlanketAddBtn = newButton.GetComponent<BlanketAddBtn>();
            newBlanketAddBtn.AddPanel = this;
            newBlanketAddBtn.blanketScript = gameManager.Get_Blanket(blanketName);
            newBlanketAddBtn.BlanketCount = count;
            newBlanketAddBtn.Set_BlanketCount(count);

            BlanketAddBtnDic.Add(blanketName, newBlanketAddBtn);
        }
    }

    public void Delete_In_BlanketAddBtnDic(string blanketName)
    {
        BlanketAddBtnDic.Remove(blanketName);
    }

    // 인벤토리 수량 변화할 때마다 UI도 반응해서 변경해주는 메서드
    private void BlanketInvenChanged(string blanketName, int invenDelta)
    {
        if (invenDelta == 0) return;

        if(invenDelta > 0) // 재고량이 늘었음 (이불장 패널에서 선택된 이불이 삭제되면서 인벤토리에 추가됨)
        {
            Add_BlanketAddBtn(blanketName, invenDelta);
        }
        else // 재고량이 줄었음 (인벤토리 패널에서 선택된 이불이 이불장으로 들어감)
        {
            BlanketAddBtnDic[blanketName].Change_BlanketCount(invenDelta); // delta -> invenData: 줄어든 만큼의 음수
        }
    }
}