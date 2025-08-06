using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SnacksPanel : MonoBehaviour
{
    [Header("Snack Data")]
    public SnacksData[] itemDatas;

    [Header("Slot References")]
    public Transform scrollContent;
    public SnacksInventory snacksInventory;
    public StoragePanel storagePanel;

    void Start()
    {
        if (snacksInventory == null)
        {
            snacksInventory = FindObjectOfType<SnacksInventory>();
        }

        if (storagePanel == null)
        {
            storagePanel = FindObjectOfType<StoragePanel>();
        }

        // 이벤트 등록
        if (snacksInventory != null)
        {
            snacksInventory.OnInventoryChanged.AddListener(RefreshUI);
        }

        // 최초 갱신
        StartCoroutine(WaitForSlotsAndApply());

        storagePanel.InitScroll();

    }

    void OnEnable()
    {
        // 패널이 꺼졌다 켜졌을 때도 갱신되도록
        if (snacksInventory == null)
            snacksInventory = FindObjectOfType<SnacksInventory>();

        if (storagePanel == null)
            storagePanel = FindObjectOfType<StoragePanel>();

        StartCoroutine(WaitForSlotsAndApply());
    }

    void RefreshUI()
    {
        itemDatas = snacksInventory.ownedSnacks
            .Where(e => e != null && e.data != null && e.count > 0)
            .Select(e => e.data)
            .ToArray();

        ApplyDataToSlots();
    }

    public void ApplyDataToSlots()
    {
        int slotCount = scrollContent.childCount;

        for (int i = 0; i < slotCount; i++)
        {
            Transform slot = scrollContent.GetChild(i);
            SnackSlotUI ui = slot.GetComponent<SnackSlotUI>();
            ItemDrag drag = slot.GetComponentInChildren<ItemDrag>();

            if (i < itemDatas.Length && itemDatas[i] != null)
            {
                var data = itemDatas[i];
                int count = snacksInventory.GetCount(data);

                if (count > 0)
                {
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

    IEnumerator WaitForSlotsAndApply()
    {
        // 슬롯이 준비될 때까지 대기
        while (scrollContent.childCount < storagePanel.itemCount)
        {
            yield return null;
        }

        RefreshUI();
    }
}
