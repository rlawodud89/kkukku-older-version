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

    private int BlanketCount;
    private BlanketAddBtn SelectedBtn;

    // Start is called before the first frame update
    void Start()
    {
        BlanketCount = 5;

        InitScroll();
    }

    // Update is called once per frame
    void Update()
    {

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
            if (SelectedBtn.Change_BlanketCount(-input_count))
            {
                TablePanel.Add_BlanketBtn(SelectedBtn, input_count);
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
        for (int i = 0; i < BlanketCount; i++)
        {
            GameObject newButton = Instantiate(BlanketBtn, ScrollContent);
            BlanketAddBtn newBlanketAddBtn = newButton.GetComponent<BlanketAddBtn>();
            newBlanketAddBtn.AddPanel = this;
            newBlanketAddBtn.Set_BlanketCount(5);
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
        SelectImg.sprite = AfterBtn.BtnImageSprite; //이미지 교체
        //선택된 이미지 보이게 만들기
        color.a = 1f; //불투명하게
        SelectImg.color = color;
    }
}