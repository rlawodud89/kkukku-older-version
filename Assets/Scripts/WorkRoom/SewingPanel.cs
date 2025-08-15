using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SewingPanel : MonoBehaviour
{
    [Header("Slot References")]
    public Transform scrollContent;

    public StoragePanel storagePanel;
    public GameObject BallonPanel;

    public ItemScript currentSewing;

    private GameManager gameManager;
    private void Start()
    {
        if (storagePanel == null)
        {
            storagePanel = FindObjectOfType<StoragePanel>();
        }

        gameManager = GameManager.getInstance();

        RefreshInventoryUI();
    }

    public void SetSelectedBlanket()
    {

        gameManager = GameManager.getInstance();


        Debug.Log(currentSewing);
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

    private void RefreshInventoryUI()
    {
        if (gameManager == null)
            gameManager = GameManager.getInstance();

        List<(ItemScript item, int count)> cottonInventory = gameManager.Get_Cotton_Inventory();

        if (!storagePanel.isInit)
        {
            storagePanel.InitScroll();
            storagePanel.isInit = true;
        }

        if (scrollContent == null)
            scrollContent = storagePanel.ScrollContent;

        for (int i = 0; i < scrollContent.childCount; i++)
        {
            var slot = scrollContent.GetChild(i);
            var ui = slot.GetComponent<BlanketSlotUI>();

            if (ui != null)
            {
                if (i < cottonInventory.Count)
                {
                    var data = cottonInventory[i];
                    ui.SetData(data.item, data.count);
                }
                else
                {
                    ui.ClearSlot();
                }
            }
        }
    }
}
