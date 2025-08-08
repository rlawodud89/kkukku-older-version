using UnityEngine;
using UnityEngine.UI;

public class BlanketSlotUI : MonoBehaviour
{
    public SlotType slotType; // 프리팹에서 설정
    public Button button;
    public Text countText;
    public GameObject checkPanel;

    private ItemScript currentData;
    private int count = 0;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnSlotButtonClicked);
        }
    }

    public bool HasData(ItemScript data) => currentData == data;
    public bool HasAnyData() => currentData != null;

    public void SetData(ItemScript data)
    {
        if (data == null) return;

        if (currentData == data)
        {
            count += 1;
            Debug.Log($"{data.itemName} count 증가: {count}");
        }
        else
        {
            currentData = data;
            count = 1;

            if (button != null)
            {
              //  button.image.sprite = slotType == SlotType.Cotton ? currentData.Fabric : currentData.Cotton;
            }

            Debug.Log($"[{slotType}] 슬롯에 새 데이터 세팅: {currentData.itemName}");
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

    private void OnSlotButtonClicked()
    {
        if (currentData == null) return;

        if (checkPanel != null)
        {
            checkPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CheckPanel이 연결되지 않았습니다.");
        }
    }

    public void OnMakeButtonClicked()
    {
        if (currentData == null) return;

        checkPanel.SetActive(false);

        Debug.Log($"[{slotType}] 슬롯에서 Make 클릭: {currentData.itemName}");

        if (slotType == SlotType.Cotton)
        {
            Make_Cotton.Instance?.HandleMakeClicked(currentData);
        }
        else if (slotType == SlotType.Sewing)
        {
            Make_Sewing.Instance?.HandleMakeClicked(currentData);
        }

        ClearSlot();
    }
}
