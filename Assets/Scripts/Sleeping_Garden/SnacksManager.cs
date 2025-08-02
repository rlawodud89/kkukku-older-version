using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnacksManager : MonoBehaviour
{
    public List<SnacksData> SnacksList;

    public static SnacksManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public List<SnacksData> GetSnacksList()
    {
        return SnacksList;
    }
}