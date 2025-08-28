using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialUISlotBtn : MonoBehaviour
{
    public Button button;                // 슬롯 버튼 (Image 포함)
    public TextMeshProUGUI countTexts;

    public Image Text_image;
    private ItemScript currentData;
    private int count = 0;



    public void SetData((ItemScript item, int count) data)
    {
        if (data.item == null || data.count <= 0)
        {
            ClearSlots();
            return;
        }

        currentData = data.item;
        count = data.count;

        if (button != null)
        {
            button.image.sprite = currentData.image; // 버튼 이미지 변경
            button.gameObject.SetActive(true);       // 보이게
        }

        if (Text_image != null)
            Text_image.gameObject.SetActive(true); // ✅ 비활성화

        UpdateCountText();
    }

    public void ClearSlots()
    {
        currentData = null;
        count = 0;

        if (button != null)
        {
            button.image.sprite = null;
            button.gameObject.SetActive(false);      // ❌ 슬롯 안 보이게
        }

        if (countTexts != null)
            countTexts.text = "";

        if (Text_image != null)
            Text_image.gameObject.SetActive(false); // ✅ 비활성화
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
