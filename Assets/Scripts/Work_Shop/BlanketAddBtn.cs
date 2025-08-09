using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlanketAddBtn : ScrollBtn
{
    public TableAddPanel AddPanel;


    void Start()
    {
        blanketImage.sprite = blanketScript.image;
        CountText.text = BlanketCount.ToString();
        Set_NotSelected();
    }

    public void ClickBtn()
    {
        if (selected) //선택 -> 선택 X 상태로
        {
            AddPanel.Chanage_SelectedBtn(null);
        }
        else //선택 X -> 선택 상태로
        {
            AddPanel.Chanage_SelectedBtn(this);
        }
    }

    public bool Change_BlanketCount(int delta)
    {
        if (delta < 0 && BlanketCount + delta < 0)
        {
            Debug.Log("수량보다 많이 추가");
            return false;
        }

        BlanketCount += delta;

        if (BlanketCount <= 0)
        {
            AddPanel.Delete_In_BlanketAddBtnDic(blanketScript.itemName);
            Destroy(this.gameObject);
        }
        else
        {
            CountText.text = BlanketCount.ToString();
        }

        return true;
    }
}
