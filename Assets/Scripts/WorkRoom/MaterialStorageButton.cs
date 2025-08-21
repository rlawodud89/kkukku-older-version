using System.Collections.Generic;
using UnityEngine;

public class MaterialStorageButton : MonoBehaviour
{
    public StoragePanel storagePanel;
    public Transform scrollContent; // 슬롯들이 있는 부모 오브젝트

    private GameManager gameManager;
    private void Start()
    {
        if (storagePanel == null)
        {
            storagePanel = FindObjectOfType<StoragePanel>();
        }

        if (!storagePanel.isInit)
        {
            storagePanel.InitScroll();
            storagePanel.isInit = true;
        }

        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
            Debug.Log("[MaterialStorage] Start - GameManager 인스턴스 가져옴");
        }
        if (scrollContent == null)
        {
            scrollContent = storagePanel.ScrollContent;
        }

    }

    private void OnEnable()
    {
        RefreshUIFromInventory();
    }

    private void RefreshUIFromInventory()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        if (gameManager == null)
        {
            Debug.LogError("[MaterialStorage] GameManager 인스턴스를 가져오지 못했습니다!");
            return;
        }


        List<(ItemScript item, int count)> dataList = gameManager.Get_Material_Inventory();

        Debug.Log($"[MaterialStorage] 인벤토리에서 가져온 데이터 개수: {dataList.Count}");

        for (int i = 0; i < dataList.Count; i++)
        {
            Debug.Log($"[MaterialStorage] {i}번 아이템: {dataList[i].item?.name}, 개수: {dataList[i].count}");
        }

        RefreshUI(dataList);
    }

    private void RefreshUI(List<(ItemScript item, int count)> dataList)
    {
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
            var ui = slot.GetComponent<MaterialUISlotBtn>();

            if (ui != null)
            {
                if (i < dataList.Count)
                {
                    ui.SetData(dataList[i]);  // 새 메서드 호출
                }
                else
                {
                    ui.ClearSlots();
                }
            }

        }
    }
}
