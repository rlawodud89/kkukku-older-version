using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TablePanel : MonoBehaviour
{
    public TableAddPanel AddPanel;

    public Transform ScrollContent;
    public GameObject BlanketBtn;

    private int BlanketCount;
    private BlanketBtn SelectedBtn;

    void Start()
    {
        AddPanel.gameObject.SetActive(false);
        BlanketCount = 5;

        InitScroll();
    }

    void Update()
    {
        
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
        Destroy(SelectedBtn.gameObject);
        SelectedBtn = null;
        BlanketCount--;
    }

    private void InitScroll()
    { 
        for (int i = 0; i < BlanketCount; i++)
        {
            GameObject newButton = Instantiate(BlanketBtn, ScrollContent);
            BlanketBtn newBlanketBtn = newButton.GetComponent<BlanketBtn>();
            newBlanketBtn.TablePanel = this;
        }
    }

    public void Chanage_SelectedBtn(BlanketBtn AfterBtn)
    {
        if(AfterBtn == null) 
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

    public void Add_BlanketBtn(BlanketAddBtn AddBtn, int count)
    {
        GameObject newButton = Instantiate(BlanketBtn, ScrollContent);

        BlanketBtn newBlanketBtn = newButton.GetComponent<BlanketBtn>();
        newBlanketBtn.TablePanel = this;
        newBlanketBtn.BtnImageSprite = AddBtn.BtnImageSprite;
        newBlanketBtn.Set_BlanketCount(count);

        Image newBtnImage = newButton.GetComponent<Image>();
        newBtnImage.sprite = AddBtn.BtnImageSprite;
    }

}
