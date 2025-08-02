using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Clock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoursInGameDay = 24f;
    private float totalGameTime=0;

    private Transform clockHandTransform;
    private GameObject timeTextObject;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        clockHandTransform = transform.Find("ClockHand");
        clockHandTransform.localRotation = Quaternion.Euler(0, 0, 46);

        timeTextObject = transform.Find("TimeText")?.gameObject;

        InvokeRepeating("AddOneMinute", 0f, 1.25f);
        //InvokeRepeating("AddOneMinute", 0f, 0.05f); 

        gameManager = GameManager.getInstance();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AddOneMinute()
    {
        //totalGameTime += 1;  // 1분을 증가

        //// 게임 내 하루를 24시간으로 나누어 실제 시간으로 변환
        //float gameHour = (totalGameTime / 60f) % hoursInGameDay;  // 60분당 1시간
        //float gameMinutes = (totalGameTime % 60);  // 1시간을 넘어가지 않도록 60분으로 나눈 나머지

        int gameHour = gameManager.Get_Hours();
        int gameMinutes = gameManager.Get_Minutes();

        // 게임 시간을 시, 분 형식으로 포맷
        string gameTimeFormatted = string.Format("{0:D2}시 {1:D2}분", (int)gameHour, (int)gameMinutes);

        // 게임 시간 출력
        //Debug.Log("게임 시간: " + gameTimeFormatted);

        TextMeshProUGUI timeText = this.GetComponentInChildren<TextMeshProUGUI>();
        if (timeText != null)
        {
            timeText.text = gameTimeFormatted;  // UI에 게임 시간 표시
        }

        // 시계 바늘 회전 각도 계산
        float angle = -gameHour * (360f / hoursInGameDay);

        // 시계 바늘 회전
        if (clockHandTransform != null)
        {
            clockHandTransform.localRotation = Quaternion.Euler(0, 0, 46 + angle);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (timeTextObject != null)
        {
            timeTextObject.SetActive(true); 
        }

        //Debug.Log("마우스가 시계 위에 있습니다.");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (timeTextObject != null)
        {
            timeTextObject.SetActive(false);
        }

       //Debug.Log("마우스가 시계를 벗어났습니다.");
    }


}
