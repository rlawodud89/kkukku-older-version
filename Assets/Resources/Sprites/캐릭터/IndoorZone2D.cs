// IndoorZoneByFoot2D.cs (새 파일)
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider2D))]
public class IndoorZone2D : MonoBehaviour
{
    [Header("Targets (same Sorting Layer)")]
    public SortAgentTilemap2D wall;         // 벽 타일맵 Agent
    public SortAgentSprite2D[] shelves;     // 실내 수납장들
    public SortAgentSprite2D[] alwaysBack;  // '늘 뒤' 그룹(벽과 동기화)

    [Header("Character filter (선택)")]
    public LayerMask characterMask;         // 캐릭터 레이어(비워두면 전체 허용)

    [Header("Bands (relative order)")]
    public int bandOutsideCharacter = 0;         // 실외 캐릭터
    public int bandWallWhenInside = 600_000;   // 실내>0 : 벽(중간)
    public int bandWallWhenEmpty = 900_000;   // 실내=0 : 벽(최상단)
    public int bandShelfInside = 800_000;   // 선반(실내일 때 벽보다 큼)
    public int bandCharacterInside = 1_000_000; // 실내 캐릭터(최상단)

    [Header("Wall sync options for 'AlwaysBack'")]
    public bool alwaysBackUseWallBand = true;       // 벽과 같은 밴드
    public bool alwaysBackMatchWallExactly = false; // 벽과 정확 동일 오더 맞추기
    public int alwaysBackDeltaToWall = -1;         // 동일 대신 살짝 뒤(-1 등)

    [Header("Foot判定 안정화")]
    public float checkInterval = 0.05f;    // 검사 주기(초) — 20fps 수준
    public float edgeHysteresis = 0.02f;   // 경계 흔들림 방지용 시간(초)
    public float footMargin = 0.0f;        // 경계 여유(OverlapPoint 대신 원으로 검사하고 싶으면 사용)

    // 내부 상태
    readonly HashSet<SortAgentSprite2D> _inside = new();
    readonly Dictionary<SortAgentSprite2D, float> _edgeTimers = new();
    Collider2D _zone;
    float _nextCheckTime;

    const string kCharKey = "Indoor.Char";
    const string kWallKey = "Indoor.Wall";
    const string kShelfKey = "Indoor.Shelf";
    const string kABKey = "Indoor.AlwaysBack";

    void OnEnable() { _zone = GetComponent<Collider2D>();
        RecomputeByFoot();
    }
    void Update()
    {
        if (Application.isPlaying && Time.time < _nextCheckTime) return;
        _nextCheckTime = Time.time + checkInterval;
        RecomputeByFoot();
    }

    void RecomputeByFoot()
    {
        var nowInside = new HashSet<SortAgentSprite2D>();

        foreach (var a in SortAgentSprite2D.All)
        {
            if (!a || !a.isCharacter) continue;

            // 레이어 필터(선택)
            if (characterMask.value != 0)
            {
                int bit = 1 << a.gameObject.layer;
                if ((characterMask.value & bit) == 0) continue;
            }

            Vector3 foot = a.FootWorld;

            bool inside = footMargin <= 0f
                ? _zone.OverlapPoint(foot)
                : Physics2D.OverlapCircle(foot, footMargin, 1 << gameObject.layer) == _zone; // 간단 예시

            // 경계 히스테리시스: 너무 경계에서 깜빡거리는 것 완화
            if (inside) nowInside.Add(a);
            else
            {
                // 바로 제외하지 않고 edgeHysteresis 동안 유지
                if (_inside.Contains(a))
                {
                    float t = 0f; _edgeTimers.TryGetValue(a, out t);
                    t += checkInterval;
                    if (t < edgeHysteresis) { nowInside.Add(a); _edgeTimers[a] = t; }
                    else _edgeTimers.Remove(a);
                }
            }

            if (inside && _inside.Add(a)) // 새로 들어온 경우
                a.SetModifier(kCharKey, bandCharacterInside);
        }

        // 나간 애들 처리
        var toRemove = new List<SortAgentSprite2D>();
        foreach (var prev in _inside)
        {
            if (!nowInside.Contains(prev))
            {
                prev.SetModifier(kCharKey, bandOutsideCharacter);
                prev.ClearModifier(kCharKey);
                toRemove.Add(prev);
            }
        }
        foreach (var r in toRemove) _inside.Remove(r);

        // 벽/선반/늘뒤 적용
        ApplyBands();
    }

    void ApplyBands()
    {
        if (wall)
        {
            if (_inside.Count > 0)
            {
                wall.SetModifier(kWallKey, bandWallWhenInside);
            }
            else
            {
                wall.SetModifier(kWallKey, bandWallWhenEmpty);
            }
        }

        if (shelves != null)
            foreach (var s in shelves) if (s)
                    s.SetModifier(kShelfKey, _inside.Count > 0 ? bandShelfInside : bandOutsideCharacter);

        if (alwaysBack != null && alwaysBack.Length > 0 && wall)
        {
            if (alwaysBackMatchWallExactly)
            {
                int wallOrder = wall.PreviewOrder(true);
                foreach (var ab in alwaysBack) if (ab)
                    {
                        int baseOrder = ab.PreviewOrder(false);
                        int needed = (wallOrder + alwaysBackDeltaToWall) - baseOrder;
                        ab.SetModifier(kABKey, needed);
                    }
            }
            else if (alwaysBackUseWallBand)
            {
                int wallBand = (_inside.Count > 0 ? bandWallWhenInside : bandWallWhenEmpty);
                foreach (var ab in alwaysBack) if (ab) ab.SetModifier(kABKey, wallBand);
            }
            else
            {
                foreach (var ab in alwaysBack) if (ab) { ab.SetModifier(kABKey, bandOutsideCharacter); ab.ClearModifier(kABKey); }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!_zone) _zone = GetComponent<Collider2D>();
        if (!_zone) return;

        Gizmos.color = new Color(0, 1, 0, 0.1f);
        var bc = _zone as BoxCollider2D;
        if (bc)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(bc.offset, bc.size);
        }
        // 필요시 Circle/Polygon도 추가

        // 현재 프레임 내부 캐릭터들의 foot 표시
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = new Color(1, 0.8f, 0, 0.8f);
        foreach (var a in SortAgentSprite2D.All)
        {
            if (!a || !a.isCharacter) continue;
            var p = a.FootWorld;
            Gizmos.DrawWireSphere(p, 0.05f + footMargin);
        }
    }
#endif
}