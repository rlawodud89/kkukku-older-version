using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MaterialsInventory : MonoBehaviour
{
    public List<MaterialInventoryEntry> ownedMaterials;

    public UnityEvent OnInventoryChanged = new UnityEvent();

    public Dictionary<ItemScript, int> GetAllMaterial()
    {
        var dict = new Dictionary<ItemScript, int>();
        foreach (var entry in ownedMaterials)
        {
            if (entry.data != null)
            {
                dict[entry.data] = entry.count;
            }
        }
        return dict;
    }

    public int GetCount(ItemScript data)
    {
        var entry = ownedMaterials.Find(e => e.data == data);
        return entry != null ? entry.count : 0;

    }

    public void AddMaterial(ItemScript material, int amount = 1)
    {
        if (material == null || amount <= 0) return;

        var entry = ownedMaterials.Find(e => e.data == material);
        if (entry != null)
        {
            entry.count += amount;
        }
        else
        {
            ownedMaterials.Add(new MaterialInventoryEntry
            {
                data = material,
                count = amount
            });
        }

        // UI 갱신을 위한 이벤트 호출
        OnInventoryChanged?.Invoke();
    }
}
