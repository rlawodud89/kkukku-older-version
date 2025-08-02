using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CottonPanel : MonoBehaviour
{

    [Header("Slot References")]
    public Transform scrollContent;   // 20개 고정 슬롯 미리 있어야 함

    public StoragePanel storagePanel;

    public GameObject BallonPanel;


    public BlanketData currentBlanket;

    void Start()
    {
        // 초기화
        storagePanel.InitScroll();
    }


    public void SetSelectedBlanket(BlanketData blanket)
    {
        currentBlanket = blanket;
        RefreshSelectedBlanketUI();
    }



    void RefreshSelectedBlanketUI()
    {
        if (currentBlanket == null)
        {
            // currentBlanket 없으면 슬롯 전부 클리어 (필요시)
            for (int i = 0; i < scrollContent.childCount; i++)
            {
                var slot = scrollContent.GetChild(i);
                var ui = slot.GetComponent<CottonSlotUI>();
                ui?.ClearSlot();
            }
            return;
        }

        // 1) 같은 데이터 가진 슬롯 찾기
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            var slot = scrollContent.GetChild(i);
            var ui = slot.GetComponent<CottonSlotUI>();
            if (ui == null) continue;

            if (ui.HasData(currentBlanket))  // HasData는 아래에서 설명
            {
                // 겹치는 슬롯 찾음 -> count +1
                ui.SetData(currentBlanket);  // 내부에서 count 올림 처리
                Debug.Log("기존 슬롯에 count +1");
                return;  // 처리 끝
            }
        }

        // 2) 겹치는 슬롯 없으면 빈 슬롯 찾아 새로 세팅
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            var slot = scrollContent.GetChild(i);
            var ui = slot.GetComponent<CottonSlotUI>();
            if (ui == null) continue;

            if (!ui.HasAnyData())  // 빈 슬롯 체크
            {
                ui.SetData(currentBlanket); // 새 데이터 세팅 (count 1 이상으로)
                Debug.Log("빈 슬롯에 새 데이터 세팅");
                return;  // 처리 끝
            }
        }

        // 3) 빈 슬롯도 없으면 필요시 처리 (예: 경고 로그)
        Debug.LogWarning("빈 슬롯이 없습니다! 더 이상 추가할 수 없습니다.");
    }




}
