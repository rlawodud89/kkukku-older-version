// SortAgentTilemap2D.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
[DefaultExecutionOrder(10000)]
public class SortAgentTilemap2D : MonoBehaviour
{
    public string sortingLayer = "Characters";

    public enum DepthRole { AlwaysBack, Dynamic, AlwaysFront }
    [Header("Role")]
    public DepthRole role = DepthRole.Dynamic;

    [Header("Role tiers (same layer)")]
    public int tierBack = -2_000_000;
    public int tierDynamic = 0;
    public int tierFront = 2_000_000;

    [Header("Tilemap base")]
    public TilemapRenderer tilemapRenderer;
    public int baseOrder = 0;     // 타일맵 기준 오더
    public int localOffset = 0;

    readonly Dictionary<string, int> _mods = new();

    int _lastAppliedOrder = int.MinValue;
    DepthRole _lastRole = DepthRole.Dynamic;
    int _lastTier = 0;
    int _lastModSum = int.MinValue;

    void Reset() { tilemapRenderer = GetComponent<TilemapRenderer>(); }
    void OnEnable() { if (!tilemapRenderer) tilemapRenderer = GetComponent<TilemapRenderer>(); ApplyOrder(true); }

    void LateUpdate() => ApplyOrder(false);

    public void SetModifier(string key, int value)
    {
        if (value == 0) { if (_mods.Remove(key)) ApplyOrder(true); return; }
        _mods[key] = value; ApplyOrder(true);
    }
    public void ClearModifier(string key) { if (_mods.Remove(key)) ApplyOrder(true); }

    public int PreviewOrder(bool includeModifiers)
    {
        int baseKey = baseOrder + localOffset;
        int tier = role switch
        {
            DepthRole.AlwaysBack => tierBack,
            DepthRole.AlwaysFront => tierFront,
            _ => tierDynamic
        };
        int mod = 0; if (includeModifiers) foreach (var kv in _mods) mod += kv.Value;
        return tier + baseKey + mod;
    }

    void ApplyOrder(bool force)
    {
        int key = PreviewOrder(true);

        int tier = role switch
        {
            DepthRole.AlwaysBack => tierBack,
            DepthRole.AlwaysFront => tierFront,
            _ => tierDynamic
        };
        int modSum = 0; foreach (var kv in _mods) modSum += kv.Value;

        bool noChange = !force && key == _lastAppliedOrder && role == _lastRole && tier == _lastTier && modSum == _lastModSum;
        if (noChange) return;

        _lastAppliedOrder = key; _lastRole = role; _lastTier = tier; _lastModSum = modSum;

        if (tilemapRenderer)
        {
            tilemapRenderer.sortingLayerName = sortingLayer;
            tilemapRenderer.sortingOrder = key;
        }
    }
}