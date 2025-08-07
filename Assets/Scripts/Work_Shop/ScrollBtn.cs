using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBtn : MonoBehaviour
{
    public TextMeshProUGUI CountText;
    public Image blanketImage;
    public Outline outline;
    public ItemScript blanketScript;
    public int BlanketCount;

    protected bool selected;


    void Start()
    {
        blanketImage.sprite = blanketScript.image;
        CountText.text = BlanketCount.ToString();
        Set_NotSelected();
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
