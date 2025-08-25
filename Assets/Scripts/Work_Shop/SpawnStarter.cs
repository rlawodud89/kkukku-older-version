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
        if (isOpen)
        {
            spawner.SetActive(true);
            Spawner spawner1 = spawner.GetComponent<Spawner>();
            spawner1.StartSpawning();
        }
        else
        {
            spawner.SetActive(false);
        }
    }
}
