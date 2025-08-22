using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialMaterialUISlot : MonoBehaviour
{
    public Image image;           // 슬롯 이미지
    public TextMeshProUGUI countText; // 슬롯 수량 텍스트
    public MaterialSelectPanel materialSelectPanel;
    // 슬롯 클릭 시 패널 열기
    public void OnClickSlot()
    {
        materialSelectPanel.Open(this);
    }

    // 패널 Confirm 시 호출
    public void SetData((Sprite sprite, int count) data)
    {
        image.sprite = data.sprite;
        countText.text = data.count.ToString();
    }
}
