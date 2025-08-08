using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SnacksPanel : MonoBehaviour
{
    [Header("Slot References")]
    public Transform scrollContent;
    public SnacksInventory snacksInventory;
    public StoragePanel storagePanel;

    private List<(ItemScript item, int count)> snackDataList;

    void Start()
    {
        if (snacksInventory == null)
            snacksInventory = FindObjectOfType<SnacksInventory>();

        if (storagePanel == null)
            storagePanel = FindObjectOfType<StoragePanel>();

        StartCoroutine(WaitForSlotsAndApply());
        storagePanel.InitScroll();
    }

    void OnEnable()
    {
        StartCoroutine(WaitForSlotsAndApply());
    }

    IEnumerator WaitForSlotsAndApply()
    {
        while (scrollContent.childCount < storagePanel.itemCount)
            yield return null;

        RefreshUI();
    }

    void RefreshUI()
    {
        snackDataList = snacksInventory.GetSnackInventory()
            .Where(e => e.item != null && e.item != null && e.count > 0)
            .ToList();

        ApplyDataToSlots();
    }

    void ApplyDataToSlots()
    {
        int slotCount = scrollContent.childCount;

        for (int i = 0; i < slotCount; i++)
        {
            var slot = scrollContent.GetChild(i);
            var ui = slot.GetComponent<SnackSlotUI>();
            var drag = slot.GetComponentInChildren<ItemDrag>();

            if (i < snackDataList.Count)
            {
                var (item, count) = snackDataList[i];
                var data = item;

                ui.SetData(data, count);

                if (drag != null)
                {
                    drag.itemData = data;
                    drag.enabled = true;
                }
            }
            else
            {
                ui.ClearSlot();

                if (drag != null)
                {
                    drag.itemData = null;
                    drag.enabled = false;
                }
            }
        }
    }
}
