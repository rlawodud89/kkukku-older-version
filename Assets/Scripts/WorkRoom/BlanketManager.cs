using System.Collections.Generic;
using UnityEngine;

public class BlanketManager : MonoBehaviour
{
    public List<ItemScript> blanketList;

    public static BlanketManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public List<ItemScript> GetBlanketList()
    {
        return blanketList;
    }
}
