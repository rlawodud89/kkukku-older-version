using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlanketSlotUI : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI countText;
    public GameObject checkPanel;
    public Sprite defaultSprite;

    private ItemScript currentData;
    private int count = 0;

    private Make_Cotton makeCottonInstance;
    private Make_Sewing makeSewingInstance;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnSlotButtonClicked);
        }

        makeCottonInstance = Make_Cotton.Instance;
        makeSewingInstance = Make_Sewing.Instance;
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

    public void UpdateCountText()
    {
        countText.text = currentData != null ? count.ToString() : "";
    }

    private void OnSlotButtonClicked()
    {
        if (currentData == null) return;

        if (checkPanel != null)
        {
            // 토글 처리: 켜져 있으면 끄고, 꺼져 있으면 켜기
            bool isActive = checkPanel.activeSelf;
            checkPanel.SetActive(!isActive);
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
            if (makeCottonInstance != null)
            {
                // 재료 소모 및 UI 업데이트는 Make... 스크립트에서 처리하도록 합니다.
                makeCottonInstance.HandleMakeClicked(currentData, this);
            }
            Debug.Log("yarn");
        }

        else if (currentData.itemType == ItemType.COTTON)
        {
            if (makeSewingInstance != null)
            {
                // 재료 소모 및 UI 업데이트는 Make... 스크립트에서 처리하도록 합니다.
                makeSewingInstance.HandleMakeClicked(currentData, this);
            }
            Debug.Log("cotton");
        }

    }



}
