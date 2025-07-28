using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    public GameObject sleepingLetter; // 편지 내용 오브젝트

    // Start is called before the first frame update
    void Start()
    {
        
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

    // 편지 누르면 편지 내용 보기
    public void ShowLetterContent()
    {
        if (sleepingLetter != null)
        {
            sleepingLetter.SetActive(true);
        }

        this.gameObject.SetActive(false);
    }

    // 편지 내용 숨기기
    public void HideLetterContent()
    {
        if (sleepingLetter != null)
        {
            sleepingLetter.SetActive(false);
        }

        this.gameObject.SetActive(true);
    }


}
