using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MaterialsPanel : MonoBehaviour
{
    [Header("Material Data")]
    public ItemScript[] itemDatas;           // 간식 데이터 (외부에서 주입)

    [Header("Slot References")]
    public Transform scrollContent;          // StoragePanel이 만든 슬롯들
                                             // (20개 고정 슬롯이 미리 존재해야 함)
    public MaterialsPanel materialsPanel;
    public MaterialsInventory materialsInventory;

    public StoragePanel storagePanel;

    void Start()
    {
        if (materialsInventory == null)
        {
            materialsInventory = FindObjectOfType<MaterialsInventory>();
        }

        // 이벤트 리스너 등록
        materialsInventory.OnInventoryChanged.AddListener(RefreshUI);

        storagePanel.InitScroll();

        // itemDatas 설정
        itemDatas = materialsInventory.ownedMaterials
            .Where(e => e != null && e.data != null)
            .Select(e => e.data)
            .ToArray();
    
        ApplyDataToSlots();
    }

    
    void RefreshUI()
    {
        // 최신 데이터 반영
        itemDatas = materialsInventory.ownedMaterials
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
            fishingUISlot ui = slot.GetComponent<fishingUISlot>();

            if (i < itemDatas.Length && itemDatas[i] != null)
            {
                var data = itemDatas[i];
                int count = materialsInventory.GetCount(data);

                if (count > 0)
                {
                    ui.SetData(data, count);
                }
                else
                {
                    // count 0 이면 슬롯 비우기
                    ui.ClearSlot();
                }
            }
            else
            {
                // 슬롯 인덱스 초과하거나 데이터 없으면 비우기
                ui.ClearSlot(); 
            }
        }
    }




}
