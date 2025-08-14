// SortAgentSprite2D.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[DefaultExecutionOrder(10000)]
public class SortAgentSprite2D : MonoBehaviour
{
    public Grid grid;
    public string sortingLayer = "Characters";

    public enum DepthRole { AlwaysBack, Dynamic, AlwaysFront }
    [Header("Role")]
    public DepthRole role = DepthRole.Dynamic;

    [Header("Base order from grid")]
    public bool useScreenDownDepth = false;   // ON: -(cx+cy), OFF: -cx
    [Range(0, 5)] public int leftBias = 0;     // 같은 줄에서 -cx 보정
    public int stepPerCell = 20;
    public int stepPerFloor = 1000;
    public int localOffset = 0;

    [Header("Role tiers (same layer)")]
    public int tierBack = -2_000_000;
    public int tierDynamic = 0;
    public int tierFront = 2_000_000;

    [Header("Character flag / Foot pivot")]
    public bool isCharacter = false;          // 캐릭터면 true
    public Transform foot;                    // 발/앞-왼 기준

    // ----- 캐릭터 레지스트리(발 좌표 조회용) -----
    public static readonly HashSet<SortAgentSprite2D> All = new();

    // ----- 내부 -----
    SortingGroup sg; SpriteRenderer sr;

    // 누적 수정자(Indoor/BackZone 등 여러 소스가 동시에 더함)
    readonly Dictionary<string, int> _mods = new();

    // 캐싱(위치가 안 변해도 상태 변경시 즉시 갱신)
    Vector3Int _lastCell = new(int.MinValue, int.MinValue, int.MinValue);
    int _lastAppliedOrder = int.MinValue;
    DepthRole _lastRole = DepthRole.Dynamic;
    int _lastTier = 0;
    int _lastModSum = int.MinValue;

    void OnEnable()
    {
        if (!grid) grid = FindObjectOfType<Grid>();
        if (!foot) foot = transform;
        sg = GetComponent<SortingGroup>();
        sr = GetComponent<SpriteRenderer>();
        All.Add(this);
        ApplyOrder(true);
    }
    void OnDisable() { All.Remove(this); }

    void LateUpdate()
    {
        ApplyOrder(false);
    }

    // ---- 외부 API ----
    public void SetModifier(string key, int value)
    {
        if (value == 0) { if (_mods.Remove(key)) ApplyOrder(true); return; }
        _mods[key] = value; ApplyOrder(true);
    }
    public void ClearModifier(string key) { if (_mods.Remove(key)) ApplyOrder(true); }
    public void ClearAllModifiers() { if (_mods.Count == 0) return; _mods.Clear(); ApplyOrder(true); }

    public int PreviewOrder(bool includeModifiers)
    {
        var c = grid.WorldToCell(foot ? foot.position : transform.position);
        int baseKey = useScreenDownDepth
            ? (-(c.x + c.y)) * stepPerCell + (-c.x) * leftBias
            : (-c.x) * stepPerCell;
        baseKey += c.z * stepPerFloor + localOffset;

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
        var c = grid.WorldToCell(foot ? foot.position : transform.position);
        int key = PreviewOrder(true);

        int tier = role switch
        {
            DepthRole.AlwaysBack => tierBack,
            DepthRole.AlwaysFront => tierFront,
            _ => tierDynamic
        };
        int modSum = 0; foreach (var kv in _mods) modSum += kv.Value;

        bool noChange = !force
            && c == _lastCell && key == _lastAppliedOrder
            && role == _lastRole && tier == _lastTier && modSum == _lastModSum;
        if (noChange) return;

        _lastCell = c; _lastAppliedOrder = key;
        _lastRole = role; _lastTier = tier; _lastModSum = modSum;

        if (sg) { sg.sortingLayerName = sortingLayer; sg.sortingOrder = key; }
        else if (sr) { sr.sortingLayerName = sortingLayer; sr.sortingOrder = key; }
    }

    // 디버그용 foot 월드좌표
    public Vector3 FootWorld => foot ? foot.position : transform.position;
}

