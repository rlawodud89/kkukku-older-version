using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{

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

    // 프롤로그 다시보기
    public void ClickReplayButton()
    {
        SceneManager.LoadScene("Prolog");
    }
}