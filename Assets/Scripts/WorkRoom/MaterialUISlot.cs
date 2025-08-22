using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialUISlot : MonoBehaviour
{
    public Image image;        // 슬롯에 표시될 이미지
    public TextMeshProUGUI countTexts;
    public Sprite defaultSprite;

    private ItemScript currentData;
    private int count = 0;

    public void SetData((ItemScript item, int count) data)
    {
        if (data.item == null)
        {
            return;
        }

        currentData = data.item;
        count = data.count;

        if (image != null)
            image.sprite = currentData.image;

        UpdateCountText();

    }

    public void ClearSlots()
    {

        currentData = null;
        count = 0;

        if (image != null)
            image.sprite = defaultSprite;

        if (countTexts != null)
            countTexts.text = "";
    }

    private void UpdateCountText()
    {
        if (countTexts != null)
            countTexts.text = currentData != null ? count.ToString() : "";
    }
}
