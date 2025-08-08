using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPanelManager : MonoBehaviour
{
    private static AddPanelManager instance;
    private GameManager gameManager;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameManager = GameManager.getInstance();

    }

    // 싱글톤 패턴
    private AddPanelManager() { }
    public static AddPanelManager getInstance() { return instance; }


}
