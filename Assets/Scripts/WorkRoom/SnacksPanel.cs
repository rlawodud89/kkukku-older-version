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

        // 이벤트 구독
        if (snacksInventory != null)
            snacksInventory.OnInventoryChanged += RefreshUI;

        StartCoroutine(WaitForSlotsAndApply());
        storagePanel.InitScroll();
    }

    void OnEnable()
    {
        // 슬롯 준비 후 데이터 적용
        StartCoroutine(WaitForSlotsAndApply());
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        if (snacksInventory != null)
            snacksInventory.OnInventoryChanged -= RefreshUI;
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
            .Where(e => e.item != null && e.count > 0)
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

                if (count > 0)
                {
                    ui.SetData(item, count);
                    if (drag != null)
                    {
                        drag.itemData = item;
                        drag.enabled = true;
                    }
                }
            }
            else
            {
                ui.ClearSlot(); // 완전히 비워야 하는 경우만
                if (drag != null)
                {
                    drag.itemData = null;
                    drag.enabled = false;
                }
            }
        }
    }

}
