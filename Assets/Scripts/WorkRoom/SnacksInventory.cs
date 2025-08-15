using System.Collections.Generic;
using UnityEngine;

public class SnacksInventory : MonoBehaviour
{
    public GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.getInstance();
    }
    public List<(ItemScript item, int count)> GetSnackInventory()
    {
        return gameManager.Get_Snack_Inventory();
    }

    public int GetCount(SnacksData data)
    {
        var list = GetSnackInventory();
        var match = list.Find(e => e.item == data);
        return match != default ? match.count : 0;
    }
}
