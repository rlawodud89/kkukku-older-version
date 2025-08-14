using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnacksInventory : MonoBehaviour
{
    public List<SnackInventoryEntry> ownedSnacks;

    public UnityEvent OnInventoryChanged = new UnityEvent();


    public void GiveSnackToEmployee(SnacksData snack)
    {    
        var entry = ownedSnacks.Find(e => e.data == snack);
         
        if (entry != null && entry.count > 0)
        {
            entry.count--;
            Debug.Log(entry.count);

            if (entry.count <= 0)
            {
                ownedSnacks.Remove(entry);
            }

            // 인벤토리 갱신 이벤트 호출
            OnInventoryChanged?.Invoke();
        }
    }
          
    public Dictionary<SnacksData, int> GetAllSnacks()
    {
        var dict = new Dictionary<SnacksData, int>();
        foreach (var entry in ownedSnacks)
        {
            if (entry.data != null)
            {
                dict[entry.data] = entry.count;
            }
        }
        return dict;
    }    

    public int GetCount(SnacksData data)
    {
        var entry = ownedSnacks.Find(e => e.data == data);
        return entry != null ? entry.count : 0;

    }

    /// <summary>
    /// 간식을 인벤토리에 추가합니다. 이미 존재하면 수량 증가, 없으면 새로 추가.
    /// </summary>
    public void AddSnack(SnacksData snack, int amount = 1)
    {
        if (snack == null || amount <= 0) return;

        var entry = ownedSnacks.Find(e => e.data == snack);
        if (entry != null)
        {
            entry.count += amount;
        }
        else
        {
            ownedSnacks.Add(new SnackInventoryEntry
            {
                data = snack,
                count = amount
            });
        }

        // UI 갱신을 위한 이벤트 호출
        OnInventoryChanged?.Invoke();
    }
}
