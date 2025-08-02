using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SnacksPanel : MonoBehaviour
{
    [Header("Snack Data")]
    public SnacksData[] itemDatas;           // 간식 데이터 (외부에서 주입)

    [Header("Slot References")]
    public Transform scrollContent;          // StoragePanel이 만든 슬롯들
                                             // (20개 고정 슬롯이 미리 존재해야 함)
    public SnacksPanel snacksPanel;
    public SnacksInventory snacksInventory;

    public StoragePanel storagePanel;

    void Start()
    {
        if (snacksInventory == null)
        {
            snacksInventory = FindObjectOfType<SnacksInventory>();

            Debug.Log("SnacksInventory 찾음: " + (snacksInventory != null));
        }

        // 이벤트 리스너 등록
        snacksInventory.OnInventoryChanged.AddListener(RefreshUI);

        storagePanel.InitScroll();

        // itemDatas 설정
        itemDatas = snacksInventory.ownedSnacks
            .Where(e => e != null && e.data != null)
            .Select(e => e.data)
            .ToArray();

        ApplyDataToSlots();
    }

    void RefreshUI()
    {
        // 최신 데이터 반영
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
                    // count 0 이면 슬롯 비우기
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
                // 슬롯 인덱스 초과하거나 데이터 없으면 비우기
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
