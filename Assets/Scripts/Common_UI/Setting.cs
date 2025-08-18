using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void ClickSaveGame()
    {
        // 게임 저장 로직
    }
}