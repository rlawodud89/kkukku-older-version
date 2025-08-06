using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterManager : MonoBehaviour
{
    public GameObject letterPanel; // 편지 보관함 패널
    public GameObject sleepingLetter; // 편지 내용 오브젝트

    public GameObject scrollContent;
    public GameObject letterButtonPrefab; // 편지 아이템 프리팹

    //public GameObject letterContentPrefab; // 편지 내용 패널
    //public GameObject sleepingImagePrefab; // 잠자는 이미지 프리팹
    public GameObject letterContentPanel; // 편지 내용 패널

    public LetterSO[] letters;


    // Start is called before the first frame update
    void Start()
    {
        // 테스트
        foreach (LetterSO letter in letters)
        {
            AddLetter(letter);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddLetter(LetterSO letter)
    {
        GameObject letterButton = Instantiate(letterButtonPrefab, scrollContent.transform);
        letterButton.transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>().text = letter.title; // 편지 제목 설정

        // 버튼 클릭 이벤트
        letterButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
            OnLetterButtonClicked(letter);
        });

    }

    
    // 편지 누르면 편지 내용 보기
    public void OnLetterButtonClicked(LetterSO letter)
    {
        letterPanel.SetActive(false);
        //sleepingLetter.SetActive(true);
        letterContentPanel.SetActive(true);

        // 편지 내용 설정
        //GameObject letterContent = Instantiate(letterContentPrefab, sleepingLetter.transform);
        letterContentPanel.transform.Find("LetterText").GetComponent<TMPro.TextMeshProUGUI>().text = letter.content; 
       // letterContentPanel.transform.Find("ExitButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
        //    HideLetterContent();
        //});

        // 삽화 이미지 설정
        //GameObject sleepingImage = Instantiate(sleepingImagePrefab, sleepingLetter.transform);
        letterContentPanel.transform.Find("SleepingImage").GetComponent<UnityEngine.UI.Image>().sprite = letter.sleepingImage;

    }

    // 편지 내용 숨기기
    public void HideLetterContent()
    {
        if (letterContentPanel != null)
        {
            letterContentPanel.SetActive(false);
        }

        letterPanel.SetActive(true);
    }

    // 편지보관함 패널 열기
    public void PanelClose()
    {
        letterPanel.SetActive(false);
    }

    // 편지보관함 패널 닫기
    public void PanelOpen()
    {
        letterPanel.SetActive(true);
    }


}
