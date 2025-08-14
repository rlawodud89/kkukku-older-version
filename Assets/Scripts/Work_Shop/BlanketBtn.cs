using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlanketBtn : ScrollBtn
{
    public TablePanel TablePanel;


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
            TablePanel.Chanage_SelectedBtn(null);
        }
        else //선택 X -> 선택 상태로
        {
            TablePanel.Chanage_SelectedBtn(this);
        }
    }

    public bool Change_BlanketCount(int delta)
    {
        if (delta < 0 && BlanketCount < (-delta))
        {
            Debug.Log("수량보다 많이 추가");
            return false;
        }

        BlanketCount += delta;

        if (BlanketCount <= 0)
        {
            TablePanel.Delete_In_BlanketBtnDic(blanketScript.itemName);
            Destroy(this.gameObject);
            return true;
        }
        else
        {
            CountText.text = BlanketCount.ToString();
            return true;
        }
    }
}
