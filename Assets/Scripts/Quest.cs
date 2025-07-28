using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest : MonoBehaviour
{
    public GameObject questRewardPanel; // 퀘스트 보상 패널
    public GameObject questContentPanel; // 퀘스트 내용 패널

    // Start is called before the first frame update
    void Start()
    {
        // 자식 중 모든 Button 컴포넌트를 찾음
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            if (button.name == "QuestButton")
            {
                // 클로저 이슈 방지를 위해 변수 따로 저장
                Button capturedButton = button;
                capturedButton.onClick.AddListener(() => OnQuestButtonClicked(capturedButton));
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void PanelClose()
    {
        this.gameObject.SetActive(false);
    }

    public void PanelOpen()
    {
        this.gameObject.SetActive(true);
    }

    void OnQuestButtonClicked(Button clickedButton)
    {
        Transform resultTextTransform = clickedButton.transform.Find("ResultText");

        // 이 버튼의 자식 중 Text 컴포넌트(ResultText)를 찾는다
        TextMeshProUGUI resultText = resultTextTransform.GetComponent<TextMeshProUGUI>();
        if (resultText == null)
        {
            Debug.LogWarning("ResultText가 없습니다: " + clickedButton.name);
            return;
        }
        
        string state = resultText.text.Trim();

        if (state == "완료!")
        {
            CompleteQuest(clickedButton);
        }
        else if (state == "진행 중")
        {
            ContinueQuest(clickedButton);
        }
        else
        {
            Debug.Log($"[{clickedButton.name}] 상태 알 수 없음: {state}");
        }
    }

    // 퀘스트 완료 시
    void CompleteQuest(Button button)
    {
        //Debug.Log($"[{button.name}] 퀘스트 완료 처리!");
        questRewardPanel.SetActive(true);

        Button getButton = questRewardPanel.transform.Find("GetButton").GetComponent<Button>();
        getButton.onClick.AddListener(() => {  questRewardPanel.SetActive(false); });

        // 보상 관련 동작 추가
    }

    public void QuestRewardPanelClose()
    {
        questRewardPanel.SetActive(false);
    }

    // 퀘스트 진행 중일 때
    void ContinueQuest(Button button)
    {
        //Debug.Log($"[{button.name}] 퀘스트 계속 진행!");
        // 진행 중 관련 동작 추가
        questContentPanel.SetActive(true);

    }

    public void QuestContentPanelClose()
    {
        questContentPanel.SetActive(false);
    }
}
