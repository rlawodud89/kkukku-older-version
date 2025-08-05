using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SewingPanel : MonoBehaviour
{
    [Header("Slot References")]
    public Transform scrollContent;

    public StoragePanel storagePanel;
    public GameObject BallonPanel;

    public BlanketData currentBlanket;

    private void Start()
    {
        if (storagePanel == null)
        {
            storagePanel = FindObjectOfType<StoragePanel>();
        }
    }

    public void SetSelectedBlanket(BlanketData blanket)
    {
        currentBlanket = blanket;

        if (!storagePanel.isInit)
        {
            storagePanel.InitScroll();
            storagePanel.isInit = true;
        }

        if (scrollContent == null)
        {
            scrollContent = storagePanel.ScrollContent;
        }

        RefreshSelectedBlanketUI();
    }

    void RefreshSelectedBlanketUI()
    {
        bool foundSlot = false;

        // 1. 같은 데이터 가진 슬롯 찾기
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            var slot = scrollContent.GetChild(i);
            var ui = slot.GetComponent<BlanketSlotUI>();

            // Cotton 슬롯만 사용
            if (ui != null && ui.slotType == SlotType.Sewing && ui.HasData(currentBlanket))
            {
                ui.SetData(currentBlanket);
                foundSlot = true;
                break;
            }
        }

        // 2. 빈 Cotton 슬롯 찾아서 세팅
        if (!foundSlot)
        {
            for (int i = 0; i < scrollContent.childCount; i++)
            {
                var slot = scrollContent.GetChild(i);
                var ui = slot.GetComponent<BlanketSlotUI>();

                if (ui != null && ui.slotType == SlotType.Sewing && !ui.HasAnyData())
                {
                    ui.SetData(currentBlanket);
                    foundSlot = true;
                    break;
                }
            }
        }

        // 3. 슬롯 없을 때만 경고
        if (!foundSlot)
        {
            Debug.LogWarning("빈 Sewing 슬롯이 없습니다! 더 이상 추가할 수 없습니다.");
        }
    }
}
