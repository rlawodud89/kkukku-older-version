using System;
using System.Collections.Generic;
using UnityEngine;

public class SnacksInventory : MonoBehaviour
{
    public GameManager gameManager;
    public event Action OnInventoryChanged;

    private void Start()
    {
        gameManager = GameManager.getInstance();
    }
    public List<(ItemScript item, int count)> GetSnackInventory()
    {
        return gameManager.Get_Snack_Inventory();
    }

    public int GetCount(ItemScript data)
    {
        var list = GetSnackInventory();
        var match = list.Find(e => e.item == data);
        return match != default ? match.count : 0;
    }

    public void GiveSnackToEmployee(ItemScript item)
    {
        // 개수 차감 시도
        bool success = GameManager.getInstance().Use_InventoryItem(item.itemName, 1);

        if (success)
        {
            Debug.Log($"간식 지급 완료: {item.itemName}");
            OnInventoryChanged?.Invoke(); // UI 갱신
        }
        else
        {
            Debug.LogWarning($"간식 지급 실패: {item.itemName} (재고 부족)");
        }
    }


}
