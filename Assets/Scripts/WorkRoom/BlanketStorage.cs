using System.Collections.Generic;
using UnityEngine;

public class BlanketStorage : MonoBehaviour
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
            Debug.Log("[BlanketStorage] Start - GameManager 인스턴스 가져옴");
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

        List<(ItemScript item, int count)> dataList = gameManager.Get_Blanket_Inventory();
        Debug.Log($"[BlanketStorage] 인벤토리에서 가져온 데이터 개수: {dataList.Count}");

        for (int i = 0; i < dataList.Count; i++)
        {
            Debug.Log($"[BlanketStorage] {i}번 아이템: {dataList[i].item?.name}, 개수: {dataList[i].count}");
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
            var ui = slot.GetComponent<MaterialUISlot>();

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
