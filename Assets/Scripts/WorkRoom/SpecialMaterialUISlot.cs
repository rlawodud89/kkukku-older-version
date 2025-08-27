using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialMaterialUISlot : MonoBehaviour
{
    public Image image;           // 슬롯 이미지
    public TextMeshProUGUI countText; // 슬롯 수량 텍스트
    public MaterialSelectPanel materialSelectPanel;
    public Sprite defaultImage;

    public ItemScript item;
    public int count;
    public Action OnSlotChanged;

    void Start()
    {
        ClearData();
    }

    // 슬롯 클릭 시 패널 열기
    public void OnClickSlot()
    {
        materialSelectPanel.Open(this);
    }

    // 패널 Confirm 시 호출
    public void SetData((ItemScript item, Sprite sprite, int count) data)
    {
        item = data.item;
        image.sprite = data.sprite;
        countText.text = data.count.ToString();
        count = data.count;
        OnSlotChanged?.Invoke();
    }

    public void ClearData()
    {
        item = null;
        image.sprite = defaultImage;
        countText.text = null;
        count = 0;
    }
}
