using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeechControllerTMP : MonoBehaviour
{
    [Header("UI References")]
    public GameObject speechBubble;
    public TextMeshProUGUI speechText;
    public Button triggerButton;

    [Header("대사 설정")]
    [TextArea] public string greetingText = "안녕하세요! 준비되셨나요?";
    [TextArea] public string triggerButtonText = "이 버튼을 누르셨군요!";
    [TextArea] public string idleText = "한동안 아무 동작이 없네요...";

    [Header("설정")]
    public float speechDuration = 3f;
    public float idleTimeThreshold = 10f;

    private float idleTimer = 0f;

    void Start()
    {
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(OnTriggerButtonClicked);
        }
    }

    void OnEnable()
    {
        ShowSpeech(greetingText); // 패널이 활성화될 때 인삿말 출력
        idleTimer = 0f; // 활성화될 때 타이머도 초기화
    }

    void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTimeThreshold)
        {
            ShowSpeech(idleText);
            idleTimer = 0f;
        }

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            idleTimer = 0f;
        }
    }

    void OnTriggerButtonClicked()
    {
        ShowSpeech(triggerButtonText);
        idleTimer = 0f;
    }

    public void ShowSpeech(string message)
    {
        StopAllCoroutines();
        speechText.text = message;
        speechBubble.SetActive(true);
        StartCoroutine(HideSpeechAfterDelay());
    }

    System.Collections.IEnumerator HideSpeechAfterDelay()
    {
        yield return new WaitForSeconds(speechDuration);
        speechBubble.SetActive(false);
    }
}
