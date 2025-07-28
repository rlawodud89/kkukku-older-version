using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBtn : MonoBehaviour
{
    public TextMeshProUGUI CountText;
    public Outline outline;
    public Sprite BtnImageSprite;

    protected int BlanketCount;
    protected bool selected;

    protected SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); //이미지 변경할 때 사용
        //BlanketCount = 5;
        CountText.text = BlanketCount.ToString();
        Set_NotSelected();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Set_Selected()
    {
        selected = true;
        outline.enabled = true;
    }

    public void Set_NotSelected()
    {
        selected = false;
        outline.enabled = false;
    }

    public bool Change_BlanketCount(int delta)
    {
        if(delta < 0 && BlanketCount < (-delta))
        {
            Debug.Log("수량보다 많이 추가");
            return false;
        }
        
        BlanketCount += delta;

        if(BlanketCount <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            CountText.text = BlanketCount.ToString();
        }

        return true;
    }

    public void Set_BlanketCount(int count)
    {
        if(count >= 0)
        {
            BlanketCount = count;
            CountText.text = BlanketCount.ToString();
        }
        else
        {
            Debug.Log("음수 BlanketCount");
        }
    }
}
