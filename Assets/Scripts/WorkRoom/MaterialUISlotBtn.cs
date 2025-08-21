using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialUISlotBtn : MonoBehaviour
{
    public Button button;                // 슬롯 버튼 (Image 포함)
    public TextMeshProUGUI countTexts;

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

        if (button != null)
            button.image.sprite = currentData.image; // 버튼의 Image 변경

        UpdateCountText();
    }

    public void ClearSlots()
    {
        currentData = null;
        count = 0;

        if (button != null)
            button.image.sprite = null;

        if (countTexts != null)
            countTexts.text = "";
    }

    private void UpdateCountText()
    {
        if (countTexts != null)
            countTexts.text = currentData != null ? count.ToString() : "";
    }

    public void OnClickSlot()
    {
        if (currentData != null)
        {
            MaterialSelectPanel.Instance.SetSelectedItem(currentData, count);
        }
    }

    public ItemScript GetItemScript() => currentData;
}
