using UnityEngine;
using UnityEngine.UI;

public class CottonSlotUI : MonoBehaviour
{
    public Button button;
    public Text countText;

    private BlanketData currentData;
    private int count = 0;

    public bool HasData(BlanketData data) => currentData == data;
    public bool HasAnyData() => currentData != null;

    public void SetData(BlanketData data)
    {
        if (data == null) return;

        if (currentData == data)
        {
            count += 1;
            Debug.Log($"{data.BlanketName} count 증가: {count}");
        }
        else
        {
            currentData = data;
            count = 1;
            if (button != null)
                button.image.sprite = currentData.Fabric;

            Debug.Log($"슬롯에 새 데이터 세팅: {currentData.BlanketName}");
        }

        UpdateCountText();
    }

    public void ClearSlot()
    {
        currentData = null;
        count = 0;

        if (button != null)
            button.image.sprite = null;

        if (countText != null)
            countText.text = "";
    }

    private void UpdateCountText()
    {
        countText.text = currentData != null ? count.ToString() : "";
    }

}
