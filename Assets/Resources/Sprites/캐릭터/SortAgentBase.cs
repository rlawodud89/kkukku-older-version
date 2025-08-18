using System.Collections.Generic;
using UnityEngine;

public abstract class SortAgentBase : MonoBehaviour
{
    public string sortingLayer = "Characters";

    public enum DepthRole { AlwaysBack, Dynamic, AlwaysFront }
    [Header("Role")]
    public DepthRole role = DepthRole.Dynamic;

    [Header("Role tier offsets (same layer)")]
    public int tierBack = -2_000_000;
    public int tierDynamic = 0;
    public int tierFront = 2_000_000;

    // 여러 컨트롤러(Indoor/BackZone 등)가 동시에 더하는 가중치
    readonly Dictionary<string, int> _mods = new();

    // 캐싱
    int _lastAppliedOrder = int.MinValue;
    int _lastTier = 0;
    int _lastModSum = int.MinValue;

    // ---- 공통 API ----
    public void SetModifier(string key, int value)
    {
        if (value == 0) { if (_mods.Remove(key)) ApplyOrder(force: true); return; }
        _mods[key] = value; ApplyOrder(force: true);
    }
    public void ClearModifier(string key) { if (_mods.Remove(key)) ApplyOrder(force: true); }
    public void ClearAllModifiers() { if (_mods.Count == 0) return; _mods.Clear(); ApplyOrder(force: true); }

    public int PreviewOrder(bool includeModifiers)
    {
        int baseKey = ComputeBaseOrder();
        int tier = role switch
        {
            DepthRole.AlwaysBack => tierBack,
            DepthRole.AlwaysFront => tierFront,
            _ => tierDynamic
        };
        int mod = 0;
        if (includeModifiers) foreach (var kv in _mods) mod += kv.Value;
        return tier + baseKey + mod;
    }

    void LateUpdate() => ApplyOrder(false);

    protected void ApplyOrder(bool force)
    {
        int key = PreviewOrder(includeModifiers: true);
        int tier = role switch
        {
            DepthRole.AlwaysBack => tierBack,
            DepthRole.AlwaysFront => tierFront,
            _ => tierDynamic
        };
        int modSum = 0; foreach (var kv in _mods) modSum += kv.Value;

        bool noChange = !force && key == _lastAppliedOrder && tier == _lastTier && modSum == _lastModSum;
        if (noChange) return;

        _lastAppliedOrder = key; _lastTier = tier; _lastModSum = modSum;
        ApplyToRenderer(key, sortingLayer);
    }

    // ---- 파생 클래스가 구현 ----
    protected abstract int ComputeBaseOrder();                 // 각 타입의 기본 오더 계산
    protected abstract void ApplyToRenderer(int order, string layer); // 실제 Renderer에 적용
}
