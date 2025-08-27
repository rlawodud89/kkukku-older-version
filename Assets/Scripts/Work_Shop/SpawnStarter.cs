using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStarter : MonoBehaviour
{
    public GameObject spawner;
    private GameManager gameManager;


    void Start()
    {
        gameManager = GameManager.getInstance();
        gameManager.OnOpenChanged += OpenChanged;

        OpenChanged(gameManager.Get_IsOpen());
    }

    private void OpenChanged(bool isOpen)
    {
        if (spawner == null || spawner.Equals(null)) return; // 이미 Destroy된 경우 안전 탈출

        if (isOpen)
        {
            spawner.SetActive(true);
        }
        else
        {
            spawner.SetActive(false);
        }
    }
}