using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomBtn : MonoBehaviour
{
    public GameObject confirmPanel;

    // Start is called before the first frame update
    void Start()
    {
        
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
        SceneManager.LoadScene("Work_Room");
    }

    public void CancelMove()
    {
        confirmPanel.SetActive(false);
    }
}
