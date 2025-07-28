using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlanketAddBtn : ScrollBtn
{
    public TableAddPanel AddPanel;

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
            AddPanel.Chanage_SelectedBtn(null);
        }
        else //선택 X -> 선택 상태로
        {
            AddPanel.Chanage_SelectedBtn(this);
        }
    }
}
