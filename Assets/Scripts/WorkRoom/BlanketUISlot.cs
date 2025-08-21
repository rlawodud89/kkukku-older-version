using UnityEngine;
using UnityEngine.UI;

public class BlanketSlotUI : MonoBehaviour
{
    public Button button;
    public Text countText;
    public GameObject checkPanel;
    public Sprite defaultSprite;

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
                button.image.sprite = currentData.image;
            }
        }

        UpdateCountText();
    }

    public void SetData(ItemScript data, int count)
    {
        if (data == null) return;

        currentData = data;
        this.count = count;

        if (button != null)
        {
            button.image.sprite = currentData.image;
        }

        UpdateCountText();
    }

    public void ClearSlot()
    {
        currentData = null;
        count = 0;

        if (button != null)
            button.image.sprite = defaultSprite;

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

        if (currentData.itemType == ItemType.YARN)
        {
            Make_Cotton.Instance?.HandleMakeClicked(currentData);
            Debug.Log("yarn");
        }
        else if (currentData.itemType == ItemType.COTTON)
        {
            Make_Sewing.Instance?.HandleMakeClicked(currentData);
            Debug.Log("cotton");
        }

        if (count > 1)
        {
            count -= 1;
            UpdateCountText();
        }
        else
        {
            ClearSlot();
        }
    }

}
