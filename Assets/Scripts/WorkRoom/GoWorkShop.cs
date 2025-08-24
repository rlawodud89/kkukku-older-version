using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoWorkShop : MonoBehaviour
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

    public void ClickWorkShop()
    {
        confirmPanel.SetActive(true);
    }

    public void MoveWorkShop()
    {
        gameManager.Set_EndScene("Work_Shop");
        SceneManager.LoadScene("Work_Shop");     
    }

    public void CancelMove()
    {
        confirmPanel.SetActive(false);
    }
}
