using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelectPanel : MonoBehaviour
{
    public static MaterialSelectPanel Instance;

    public Image SelectImg;             // 선택된 이미지 표시
    public TMP_InputField CountInput;
    public Button ConfirmBtn;
    public Button CancelBtn;

    private ItemScript selectedItem;
    private SpecialMaterialUISlot targetSlot;
    private Sprite selectedSprite;
    private GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }
    }

    void Awake()
    {
        Instance = this;

        ConfirmBtn.onClick.AddListener(OnConfirm);
        CancelBtn.onClick.AddListener(() => gameObject.SetActive(false));

        Color c = SelectImg.color;
        c.a = 0f;
        SelectImg.color = c;
    }


    public void SetSelectedItem(ItemScript item, int defaultCount)
    {
        selectedItem = item;
        selectedSprite = item.image;
        SelectImg.sprite = item.image;

        Color c = SelectImg.color;
        c.a = 1f;
        SelectImg.color = c;
    }


    public void Open(SpecialMaterialUISlot slot)
    {
        targetSlot = slot;
        gameObject.SetActive(true);
    }

    private void OnConfirm()
    {
        if (targetSlot != null && int.TryParse(CountInput.text, out int cnt))
        {
            targetSlot.SetData((selectedSprite, cnt)); // MaterialUISlot이 Sprite 기반으로 SetData 있어야 함
            CountInput.text = "";

            gameManager.Use_InventoryItem(selectedItem.itemName, cnt);

        }

        targetSlot = null;

        Color c = SelectImg.color;
        c.a = 0f;
        SelectImg.color = c;

        gameObject.SetActive(false);
    }
}
