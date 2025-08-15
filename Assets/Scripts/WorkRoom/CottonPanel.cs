using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CottonPanel : MonoBehaviour
{
    [Header("Slot References")]
    public Transform scrollContent;

    public StoragePanel storagePanel;
    public GameObject BallonPanel;

    public ItemScript currentCotton;

    private GameManager gameManager;
    void Start()
    {

        gameManager = GameManager.getInstance();
        if (storagePanel == null)
        {
            storagePanel = FindObjectOfType<StoragePanel>();
        }

        RefreshInventoryUI();
    }

    public void SetSelectedBlanket(ItemScript blanket)
    {

        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        currentCotton = gameManager.Blanket_to_Yarn(blanket.itemName); // 이불 -> 원단

        if (!storagePanel.isInit)
        {
            storagePanel.InitScroll();
            storagePanel.isInit = true;
        }

        if (scrollContent == null)
        {
            scrollContent = storagePanel.ScrollContent;
        }

        RefreshInventoryUI();
    }

    // UI 새로고침: 담요 인벤토리 데이터 불러와 슬롯에 세팅
    public void RefreshInventoryUI()
    {
        if (gameManager == null)
            gameManager = GameManager.getInstance();

        List<(ItemScript item, int count)> YarnInventory = gameManager.Get_Yarn_Inventory();
        //    public List<(ItemScript, int count)> Get_Cotton_Inventory()

        if (!storagePanel.isInit)
        {
            storagePanel.InitScroll();
            storagePanel.isInit = true;
        }

        if (scrollContent == null)
        {
            scrollContent = storagePanel.ScrollContent;
        }

        for (int i = 0; i < scrollContent.childCount; i++)
        {
            var slot = scrollContent.GetChild(i);
            var ui = slot.GetComponent<BlanketSlotUI>();

            if (ui != null)
            {
                if (i < YarnInventory.Count)
                {
                    var data = YarnInventory[i];
                    ui.SetData(data.item, data.count);  // 새 메서드 호출
                }
                else
                {
                    ui.ClearSlot();
                }
            }

        }
    }
}
