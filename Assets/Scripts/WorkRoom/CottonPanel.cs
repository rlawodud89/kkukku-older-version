using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CottonPanel : MonoBehaviour
{

    [Header("Slot References")]
    public Transform scrollContent;   // 20개 고정 슬롯 미리 있어야 함

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
        Debug.Log("CottonPanel.scrollContent: " + scrollContent?.name);
        Debug.Log("StoragePanel.ScrollContent: " + storagePanel?.ScrollContent?.name);
        Debug.Log("Slot count: " + scrollContent?.childCount);

        bool foundSlot = false;

        // 1. 같은 데이터 가진 슬롯 찾기
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            var slot = scrollContent.GetChild(i);
            var ui = slot.GetComponent<CottonSlotUI>();

            if (ui.HasData(currentBlanket))
            {
                ui.SetData(currentBlanket);
                foundSlot = true;
                break;
            }
        }

        // 2. 빈 슬롯 찾아서 세팅
        if (!foundSlot)
        {
            for (int i = 0; i < scrollContent.childCount; i++)
            {
                var slot = scrollContent.GetChild(i);
                var ui = slot.GetComponent<CottonSlotUI>();
                if (ui == null) continue;

                if (!ui.HasAnyData())
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
            Debug.LogWarning("빈 슬롯이 없습니다! 더 이상 추가할 수 없습니다.");
        }

    }




}
