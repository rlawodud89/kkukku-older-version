using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomBtn : MonoBehaviour
{
    public GameObject confirmPanel;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickBtnRoom()
    {
        confirmPanel.SetActive(true);
    }

    public void MoveWorkRoom()
    {
        gameManager.Set_EndScene("Work_Room");
        //SceneManager.LoadScene("Work_Room");
        Fader.GoConcurrent("Work_Room");
    }

    public void CancelMove()
    {
        confirmPanel.SetActive(false);
    }
}
