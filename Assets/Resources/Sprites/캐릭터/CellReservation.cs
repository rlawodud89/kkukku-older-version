using System.Collections.Generic;
using UnityEngine;

public static class CellReservation
{
    static readonly Dictionary<Vector3Int, Object> map = new();

    public static bool TryReserve(Vector3Int cell, Object owner)
    { if (map.ContainsKey(cell)) return false; map[cell] = owner; return true; }

    public static void Release(Vector3Int cell, Object owner)
    { if (map.TryGetValue(cell, out var o) && o == owner) map.Remove(cell); }

    public static void ReleaseAll(Object owner)
    { var rm = new List<Vector3Int>(); foreach (var kv in map) if (kv.Value == owner) rm.Add(kv.Key); foreach (var c in rm) map.Remove(c); }

    public static bool IsReserved(Vector3Int cell) => map.ContainsKey(cell);
}
