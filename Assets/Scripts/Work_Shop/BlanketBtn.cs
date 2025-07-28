using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlanketBtn : ScrollBtn
{
    public TablePanel TablePanel;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); //이미지 변경할 때 사용
        CountText.text = BlanketCount.ToString();
        Set_NotSelected();
    }

    // Update is called once per frame
    void Update()
    {

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
}
