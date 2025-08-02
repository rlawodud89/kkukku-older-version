using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlanketButtonPanel : MonoBehaviour
{
    public Transform ScrollContent;       // ScrollView > Viewport > Content
    public Button btnPrefab;
    public FabricDetailPanelController detailPanel;

    void Start()
    {
        InitScroll();
    }

    void InitScroll()
    {
        List<BlanketData> blanketList = BlanketManager.Instance.GetBlanketList();

        for (int i = 0; i < blanketList.Count; i++)
        {
            int index = i; // 클로저 방지
            BlanketData data = blanketList[index];

            Button btn = Instantiate(btnPrefab, ScrollContent);
            btn.name = $"BtnSlots_{index + 1}";

            Image btnImage = btn.GetComponent<Image>();

            if (btnImage != null)
            {
                btnImage.sprite =blanketList[index].BlanketSprite;  // 원하는 Sprite로 변경
            }
            // 텍스트 설정 (버튼에 Text 컴포넌트가 있다면)
            //Text btnText = btn.GetComponentInChildren<Text>();
            //if (btnText != null)
            //   btnText.text = data.fabricName;

            // 클릭 이벤트 연결
            btn.onClick.AddListener(() =>
            {
                Debug.Log("클릭됨");
                detailPanel.OpenPanel(data);
            });
        }
    }
}
