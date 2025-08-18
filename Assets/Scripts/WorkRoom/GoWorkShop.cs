using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoWorkShop : MonoBehaviour
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

    public void ClickWorkShop()
    {
        confirmPanel.SetActive(true);
    }

    public void MoveWorkShop()
    {
        SceneManager.LoadScene("Work_Shop");
    }

    public void CancelMove()
    {
        confirmPanel.SetActive(false);
    }
}
