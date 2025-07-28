using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FabricListController : MonoBehaviour
{
    public List<Button> fabricButtons;                 // 미리 Scene에 배치된 Button들
    public List<FabricData> fabricList;                // 각 버튼과 연결될 원단 데이터
    public FabricDetailPanelController detailPanel;    // 상세 패널

    private bool isClick = false;

    void Start()
    {
        for (int i = 0; i < fabricButtons.Count; i++)
        {
            int index = i; // 클로저 방지
            if (index < fabricList.Count && fabricButtons[index] != null)
            {
                FabricData data = fabricList[index];
                fabricButtons[index].onClick.RemoveAllListeners();
                fabricButtons[index].onClick.AddListener(() =>
                {
                    if (isClick)
                    {
                        detailPanel.ClosePanel();
                        isClick = false;
                    }
                    else
                    {
                        isClick = true;
                        Debug.Log($"[버튼 클릭] {data.fabricName}");
                        detailPanel.OpenPanel(data);
                    }
                });
            }
        }
    }
}
