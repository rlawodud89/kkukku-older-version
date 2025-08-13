using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class AStarMover : MonoBehaviour
{
    [Header("Grid / Foot")]
    public Grid grid;                // Isometric Z as Y
    public Transform foot;           // 발/앞-왼 기준

    [Header("Move")]
    public float moveSpeed = 2f;
    public bool allowDiagonal = false;
    public float arriveThreshold = 0.05f;
    public float waitAtShelfSeconds = 1.0f;
    public float waitAtCashierSeconds = 1.0f;

    [Header("Obstacle")]
    public LayerMask obstacleLayer;  // '발자국'만 포함
    public float cellProbeRadius = 0.3f;

    [Header("Route Points")]
    public NavPoint startPoint;      // 겹침 허용 (allowOverlap=true)
    public NavPoint[] doorPoints;    // 3개 포인트, 겹침 허용 (allowOverlap=true)
    public NavPoint[] shelfPoints;   // 수납장 다수, 포인트별 face 다름
    public NavPoint cashierPoint;    // 1개, face 가능

    [Header("Visits")]
    public int minShelfVisits = 2;
    public int maxShelfVisits = 5;
    [Range(0f, 1f)] public float buyProbability = 0.8f;


    /* ───────────── Init 주입 메서드 ───────────── */
    public void Init(Grid g,
                     NavPoint start,
                     NavPoint[] doors,
                     NavPoint[] shelves,
                     NavPoint cashier)
    {
        grid = g;
        startPoint = start;
        doorPoints = doors;
        shelfPoints = shelves;
        cashierPoint = cashier;
    }

    /* ───────────── Mono 구현 ───────────── */

    Animator anim;
    void Awake()
    {

        if (!grid) grid = FindObjectOfType<Grid>();
        if (!foot) foot = transform;

        anim = GetComponent<Animator>();
    }

    


    private Vector3Int? myReservedCell;

    // 도착 후 얼굴 방향을 잠가두는 용도
    bool _faceLockActive;
    Vector2 _faceLockBlend;

    // 애니 파라미터 적용 헬퍼
    void SetBlend(Vector2 b)
    {
        anim.SetFloat("MoveX", b.x);
        anim.SetFloat("MoveY", b.y);
    }

    // 방향 잠금: 다음 이동 시작 전까지 계속 유지
    void LockFace(Vector2 b)
    {
        _faceLockActive = true;
        _faceLockBlend = b;
        SetBlend(b);
        anim.SetFloat("Speed", 0f); // 정지 유지
    }

    // 이동 시작 시 잠금 해제
    void UnlockFace() => _faceLockActive = false;

    // 매 프레임 가장 마지막에 잠금값을 재적용(다른 스크립트가 덮어써도 복구)
    void LateUpdate()
    {
        if (_faceLockActive) SetBlend(_faceLockBlend);
    }

    // ---- 방향 매핑 헬퍼 ----
    [System.Serializable]
    public struct FaceMap
    {
        // 1) 셀 오프셋(접근/정렬용)
        public Vector3Int leftDown;   // 예: (-1, 0, 0)
        public Vector3Int rightUp;    // 예: ( 1, 0, 0)

        // Isometric Z As Y + XYZ 환경에 맞춰 Y 뒤집음
        public Vector3Int leftUp;     // ( 0, -1, 0)
        public Vector3Int rightDown;  // ( 0,  1, 0)

        // 2) 블렌드트리 좌표(애니용) — UR/DR/DL/UL 코너(±0.707)
        public Vector2 leftDownBlend;   // (-0.707, -0.707)
        public Vector2 rightUpBlend;    // ( 0.707,  0.707)
        public Vector2 leftUpBlend;     // (-0.707,  0.707)
        public Vector2 rightDownBlend;  // ( 0.707, -0.707)
    }

    [Header("Face 매핑(프로젝트 축/블렌드트리 보정)")]
    public FaceMap faceMap = new FaceMap
    {
        // 오프셋(현재값 유지)
        leftDown = new Vector3Int(-1, 0, 0),
        rightUp = new Vector3Int(1, 0, 0),
        leftUp = new Vector3Int(0, -1, 0),
        rightDown = new Vector3Int(0, 1, 0),

        // ★ 블렌드(Y 부호만 전체 뒤집기: 아래=+Y)
        leftDownBlend = new Vector2(-0.707f, 0.707f), // DL
        rightUpBlend = new Vector2(0.707f, -0.707f), // UR
        leftUpBlend = new Vector2(-0.707f, -0.707f), // UL
        rightDownBlend = new Vector2(0.707f, 0.707f), // DR
    };

    // Face → 블렌드트리 값
    Vector2 FaceToBlend(NavPoint.Face f) => f switch
    {
        NavPoint.Face.LeftDown => faceMap.leftDownBlend,
        NavPoint.Face.RightUp => faceMap.rightUpBlend,
        NavPoint.Face.LeftUp => faceMap.leftUpBlend,
        NavPoint.Face.RightDown => faceMap.rightDownBlend,
        _ => Vector2.zero
    };

    // "셀 델타(그리드 방향) → 블렌드" 매핑
    Vector2 CellDeltaToBlend(Vector3Int d)
    {
        if (d == faceMap.leftDown) return faceMap.leftDownBlend;
        if (d == faceMap.rightUp) return faceMap.rightUpBlend;
        if (d == faceMap.leftUp) return faceMap.leftUpBlend;
        if (d == faceMap.rightDown) return faceMap.rightDownBlend;

        // 예외(대각 등): 호출부에서 월드 dir로 스냅
        return Vector2.zero;
    }

    // 월드 방향 벡터(dir) → FaceMap 기반 4방 블렌드 스냅핑
    Vector2 WorldDirToBlend(Vector3 worldDir)
    {
        if (worldDir.sqrMagnitude < 1e-6f) return Vector2.zero;
        float sx = worldDir.x >= 0f ? 1f : -1f;
        float sy = worldDir.y >= 0f ? 1f : -1f;

        if (sx > 0f && sy > 0f) return faceMap.rightUpBlend;
        if (sx > 0f && sy < 0f) return faceMap.rightDownBlend;
        if (sx < 0f && sy < 0f) return faceMap.leftDownBlend;
        /* sx < 0 && sy > 0 */
        return faceMap.leftUpBlend;
    }

    

    void Start() { StartCoroutine(RunRoute()); }

    IEnumerator RunRoute()
    {
        // Start → Door
        if (startPoint) yield return MoveToPoint(startPoint, reserve: false);
        if (doorPoints != null && doorPoints.Length > 0)
            yield return MoveToPoint(FindNearestPoint(doorPoints), reserve: false);

        // 랜덤 수납장 2~5회
        int visits = Random.Range(minShelfVisits, maxShelfVisits + 1);
        for (int i = 0; i < visits; i++)
        {
            if (shelfPoints == null || shelfPoints.Length == 0) break;
            var p = shelfPoints[Random.Range(0, shelfPoints.Length)];
            if (p == null) continue;

            yield return MoveToPoint(p, reserve: !p.allowOverlap);
            yield return new WaitForSeconds(waitAtShelfSeconds);
        }

        // 구매 결정
        bool willBuy = cashierPoint && (Random.value < buyProbability);
        if (willBuy)
        {
            yield return MoveToPoint(cashierPoint, reserve: !cashierPoint.allowOverlap);
            yield return new WaitForSeconds(waitAtCashierSeconds);
        }

        // Door → Start
        if (doorPoints != null && doorPoints.Length > 0)
            yield return MoveToPoint(FindNearestPoint(doorPoints), reserve: false);
        if (startPoint) yield return MoveToPoint(startPoint, reserve: false);

        // 끝
        Destroy(gameObject);
    }

    // ---------- 이동 코어 ----------
    IEnumerator MoveToPoint(NavPoint p, bool reserve)
    {
        UnlockFace(); // ★ 다음 이동을 시작했으니 잠금 해제

        Vector3Int targetCell = grid.WorldToCell(p.transform.position);
        Vector3Int destCell = PickApproachCell(targetCell, p.GetOffsetsOrDefault(), reserve);

        // 경로
        Vector3Int startCell = grid.WorldToCell(foot.position);
        List<Vector3Int> path = AStarPathfinder.FindPath(startCell, destCell, IsBlocked, allowDiagonal, true);
        if (path == null || path.Count == 0) { ReleaseMyReservation(); yield break; }

        // 현재 셀 스킵
        int idx = (path.Count > 0 && path[0] == startCell) ? 1 : 0;

        for (; idx < path.Count; idx++)
        {
            Vector3 targetWorld = CellCenterForFoot(path[idx]);

            // 이번 세그먼트의 "셀 방향"을 읽어 블렌드에 반영
            Vector3Int fromCell = (idx > 0) ? path[idx - 1] : startCell;
            Vector3Int seg = path[idx] - fromCell;

            // 1차: 셀 델타 기반 매핑
            Vector2 segBlend = CellDeltaToBlend(seg);

            // 2차: (예외/대각 등) 월드 dir 기반 스냅 보정
            if (segBlend == Vector2.zero)
            {
                Vector3 fallbackDir = (targetWorld - foot.position);
                segBlend = WorldDirToBlend(fallbackDir);
            }

            SetBlend(segBlend);
            anim.SetFloat("Speed", 1f);

            // 위치 이동은 월드 dir로
            while (Vector2.Distance(foot.position, targetWorld) > arriveThreshold)
            {
                Vector3 dir = (targetWorld - foot.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                yield return null;
            }
            yield return null;
        }

        // 정지 + 바라보는 방향(포인트 지정)
        anim.SetFloat("Speed", 0f);

       
        ApplyFace(p);

        // 예약 해제
        ReleaseMyReservation();
    }

    Vector3Int PickApproachCell(Vector3Int targetCell, Vector3Int[] offsets, bool reserve)
    {
        // 현재 위치 기준 가까운 순
        Vector3Int me = grid.WorldToCell(foot.position);
        System.Array.Sort(offsets, (a, b) => {
            var ca = targetCell + a; var cb = targetCell + b;
            int da = Mathf.Abs(ca.x - me.x) + Mathf.Abs(ca.y - me.y);
            int db = Mathf.Abs(cb.x - me.x) + Mathf.Abs(cb.y - me.y);
            return da.CompareTo(db);
        });

        foreach (var off in offsets)
        {
            var c = targetCell + off;
            if (IsBlocked(c)) continue;
            if (reserve && CellReservation.IsReserved(c)) continue;

            // 경로 한 번 미리 검사(옵션)
            var start = grid.WorldToCell(foot.position);
            var probe = AStarPathfinder.FindPath(start, c, IsBlocked, allowDiagonal, true);
            if (probe == null || probe.Count == 0) continue;

            if (reserve)
            {
                if (!CellReservation.TryReserve(c, this)) continue;
                myReservedCell = c;
            }
            return c;
        }

        // 전부 실패 → 타겟 바로 앞(-x)에서 링 탐색
        var fallback = GetNearestReachableCell(targetCell + new Vector3Int(-1, 0, 0), 6);
        if (reserve && !CellReservation.IsReserved(fallback) && CellReservation.TryReserve(fallback, this))
            myReservedCell = fallback;
        return fallback;
    }

    // ---------- 헬퍼/유틸 ----------
    Vector3 CellCenterForFoot(Vector3Int cell)
    {
        Vector3 center = grid.GetCellCenterWorld(cell);
        Vector3 footToRoot = transform.position - foot.position;
        return center + footToRoot;
    }

    bool IsBlocked(Vector3Int cell)
    {
        Vector3 p = grid.GetCellCenterWorld(cell);
        var hit = Physics2D.OverlapCircle(p, cellProbeRadius, obstacleLayer);
        return hit != null; // 콜라이더는 절대 넘지 않음
    }

    Vector3Int GetNearestReachableCell(Vector3Int goal, int maxRing = 4)
    {
        if (!IsBlocked(goal)) return goal;
        for (int r = 1; r <= maxRing; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                int dy = r - Mathf.Abs(dx);
                var c1 = goal + new Vector3Int(dx, dy, 0);
                var c2 = goal + new Vector3Int(dx, -dy, 0);
                if (!IsBlocked(c1)) return c1;
                if (!IsBlocked(c2)) return c2;
            }
        }
        return goal;
    }

    NavPoint FindNearestPoint(NavPoint[] arr)
    {
        if (arr == null || arr.Length == 0) return null;
        float best = float.MaxValue; NavPoint pick = arr[0];
        Vector3 me = foot ? foot.position : transform.position;
        foreach (var p in arr)
        {
            if (!p) continue;
            float d = (p.transform.position - me).sqrMagnitude;
            if (d < best) { best = d; pick = p; }
        }
        return pick;
    }

    void ApplyFace(NavPoint p)
    {
        if (p == null || p.face == NavPoint.Face.Auto) return;
        LockFace(FaceToBlend(p.face));
    }

    void ReleaseMyReservation()
    {
        if (myReservedCell.HasValue)
        { CellReservation.Release(myReservedCell.Value, this); myReservedCell = null; }
    }

    void OnDisable() => CellReservation.ReleaseAll(this);
}


