using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public bool autoStart = true;

    [Header("Obstacle")]
    public LayerMask obstacleLayer;  // 길막 전용 레이어만 포함
    public float cellProbeRadius = 0.3f;

    [Header("Obstacle Probe (New)")]
    public bool ignoreTriggers = true;        // 트리거는 길막으로 보지 않기
    public bool useBoxProbe = true;           // 셀 크기 기준 박스 탐지
    [Range(0.1f, 1.2f)]
    public float boxSizeScale = 0.9f;         // 그리드 셀 대비 박스 크기 스케일

    [Header("Route Points")]
    public NavPoint startPoint;
    public NavPoint[] doorPoints;
    public NavPoint[] shelfPoints;
    public NavPoint cashierPoint;

    [Header("Visits")]
    public int minShelfVisits = 2;
    public int maxShelfVisits = 5;

    // ───────────── Shop / Payment ─────────────
    [Header("Shop / Payment")]
    [Range(0f, 1f)]
    [Tooltip("결제 금액 대비 에너지 증가 비율(금액 * 계수)")]
    public float energyPerGold = 0.02f;

    int _pendingPayment = 0;
    bool _pickedBlanket = false;
    int _buyVisitIndex = 0;

    // ───────────── Quest Mode ─────────────
    [Header("Quest Mode")]
    public bool questMode = false;          // 켜면 RunQuest() 사용
    public NavPoint questWaitPoint;         // 기다릴 포인트
    public float questWaitSeconds = 10f;    // 대기 시간
    public bool questLeaveViaDoor = true;   // 나갈 때 문 사용

    // ★ Canvas 제거: SpriteRenderer로 마커 토글
    [Header("Quest Marker (Sprite)")]
    public SpriteRenderer questMarker;          // 캐릭터 자식 아이콘
    public string questMarkerChildName = "QuestMarker";
    public bool toggleMarkerObject = true;      // true: SetActive, false: SpriteRenderer.enabled

    // 상태
    bool _questAccepted = false;

    // ───────────── Init 주입 ─────────────
    public void Init(Grid g, NavPoint start, NavPoint[] doors, NavPoint[] shelves, NavPoint cashier)
    { grid = g; startPoint = start; doorPoints = doors; shelfPoints = shelves; cashierPoint = cashier; }

    // ───────────── Mono ─────────────
    Animator anim;
    void Awake()
    {
        if (!grid) grid = FindObjectOfType<Grid>();
        if (!foot) foot = transform;
        anim = GetComponent<Animator>();

        // 스프라이트 마커 자동 연결(이름 우선)
        if (!questMarker && !string.IsNullOrEmpty(questMarkerChildName))
        {
            var t = transform.Find(questMarkerChildName);
            if (t) questMarker = t.GetComponent<SpriteRenderer>();
        }
    }

    void Start()
    {
        if (questMode) StartCoroutine(RunQuest());
        else StartCoroutine(RunRoute());
    }

    private Vector3Int? myReservedCell;

    // 도착 후 얼굴 방향 잠금
    bool _faceLockActive;
    Vector2 _faceLockBlend;

    void SetBlend(Vector2 b) { anim.SetFloat("MoveX", b.x); anim.SetFloat("MoveY", b.y); }
    void LockFace(Vector2 b) { _faceLockActive = true; _faceLockBlend = b; SetBlend(b); anim.SetFloat("Speed", 0f); }
    void UnlockFace() => _faceLockActive = false;
    void LateUpdate() { if (_faceLockActive) SetBlend(_faceLockBlend); }

    // ---- 방향 매핑 ----
    [System.Serializable]
    public struct FaceMap
    {
        public Vector3Int leftDown, rightUp, leftUp, rightDown;
        public Vector2 leftDownBlend, rightUpBlend, leftUpBlend, rightDownBlend;
    }

    [Header("Face 매핑(프로젝트 축/블렌드 보정)")]
    public FaceMap faceMap = new FaceMap
    {
        leftDown = new Vector3Int(-1, 0, 0),
        rightUp = new Vector3Int(1, 0, 0),
        leftUp = new Vector3Int(0, -1, 0),
        rightDown = new Vector3Int(0, 1, 0),

        leftDownBlend = new Vector2(-0.707f, 0.707f),
        rightUpBlend = new Vector2(0.707f, -0.707f),
        leftUpBlend = new Vector2(-0.707f, -0.707f),
        rightDownBlend = new Vector2(0.707f, 0.707f),
    };

    Vector2 FaceToBlend(NavPoint.Face f) => f switch
    {
        NavPoint.Face.LeftDown => faceMap.leftDownBlend,
        NavPoint.Face.RightUp => faceMap.rightUpBlend,
        NavPoint.Face.LeftUp => faceMap.leftUpBlend,
        NavPoint.Face.RightDown => faceMap.rightDownBlend,
        _ => Vector2.zero
    };

    Vector2 CellDeltaToBlend(Vector3Int d)
    {
        if (d == faceMap.leftDown) return faceMap.leftDownBlend;
        if (d == faceMap.rightUp) return faceMap.rightUpBlend;
        if (d == faceMap.leftUp) return faceMap.leftUpBlend;
        if (d == faceMap.rightDown) return faceMap.rightDownBlend;
        return Vector2.zero;
    }

    Vector2 WorldDirToBlend(Vector3 worldDir)
    {
        if (worldDir.sqrMagnitude < 1e-6f) return Vector2.zero;
        float sx = worldDir.x >= 0f ? 1f : -1f;
        float sy = worldDir.y >= 0f ? 1f : -1f;
        if (sx > 0f && sy > 0f) return faceMap.rightUpBlend;
        if (sx > 0f && sy < 0f) return faceMap.rightDownBlend;
        if (sx < 0f && sy < 0f) return faceMap.leftDownBlend;
        return faceMap.leftUpBlend;
    }

    // ───────────── 퀘스트 루틴 ─────────────
    IEnumerator RunQuest()
    {
        if (startPoint) yield return MoveToPoint(startPoint, reserve: false);
        if (doorPoints != null && doorPoints.Length > 0)
            yield return MoveToPoint(FindNearestPoint(doorPoints), reserve: false);
        if (questWaitPoint) yield return MoveToPoint(questWaitPoint, reserve: !questWaitPoint.allowOverlap);

        ShowMarker(true);
        float t = 0f;
        while (t < questWaitSeconds && !_questAccepted) { t += Time.deltaTime; yield return null; }
        ShowMarker(false);

        if (doorPoints != null && doorPoints.Length > 0)
            yield return MoveToPoint(FindNearestPoint(doorPoints), reserve: false);
        if (startPoint) yield return MoveToPoint(startPoint, reserve: false);
        Destroy(gameObject);
    }

    // ───────────── 일반 손님 루틴 ─────────────
    IEnumerator RunRoute()
    {
        if (startPoint) yield return MoveToPoint(startPoint, reserve: false);
        if (doorPoints != null && doorPoints.Length > 0)
            yield return MoveToPoint(FindNearestPoint(doorPoints), reserve: false);

        int visits = Random.Range(minShelfVisits, maxShelfVisits + 1);
        _buyVisitIndex = (visits > 0) ? Random.Range(1, visits + 1) : 0;

        for (int i = 1; i <= visits; i++)
        {
            if (shelfPoints == null || shelfPoints.Length == 0) break;
            var p = shelfPoints[Random.Range(0, shelfPoints.Length)];
            if (p == null) continue;

            yield return MoveToPoint(p, reserve: !p.allowOverlap);
            yield return new WaitForSeconds(waitAtShelfSeconds);

            if (!_pickedBlanket && _buyVisitIndex > 0 && i == _buyVisitIndex)
                TryPickBlanketAtPoint(p);
        }

        if (_pendingPayment > 0)
            yield return PayAtCashier();

        if (doorPoints != null && doorPoints.Length > 0)
            yield return MoveToPoint(FindNearestPoint(doorPoints), reserve: false);
        if (startPoint) yield return MoveToPoint(startPoint, reserve: false);
        Destroy(gameObject);
    }

    // ───────────── 쇼핑/결제 보조 ─────────────
    void TryPickBlanketAtPoint(NavPoint p)
    {
        if (!p || p.tableId <= 0) return;
        var gm = GameManager.getInstance();
        if (gm == null) { Debug.LogWarning("GameManager 인스턴스가 없습니다."); return; }

        int price = gm.Use_RandomOne_BlanketInTable(p.tableId);
        if (price > 0)
        {
            _pendingPayment += price;
            _pickedBlanket = true;
            Debug.Log($"[AStarMover] Picked blanket: table {p.tableId}, price {price}");
        }
    }

    IEnumerator PayAtCashier()
    {
        if (_pendingPayment <= 0) yield break;
        if (!cashierPoint) yield break;

        yield return MoveToPoint(cashierPoint, reserve: !cashierPoint.allowOverlap);
        yield return new WaitForSeconds(waitAtCashierSeconds);

        var gm = GameManager.getInstance();
        if (gm != null)
        {
            gm.Change_Gold(_pendingPayment);
            int energyDelta = Mathf.RoundToInt(_pendingPayment * energyPerGold);
            if (energyDelta != 0) gm.Change_Energy(energyDelta);
        }

        _pendingPayment = 0;
    }

    // ───────────── 마커 표시(스프라이트) ─────────────
    public void AcceptQuest() { _questAccepted = true; ShowMarker(false); }
    void OnMouseDown() { if (questMode) AcceptQuest(); }

    void ShowMarker(bool on)
    {
        if (!questMode) return;

        if (!questMarker)
        {
            Debug.LogWarning("[AStarMover] questMarker가 비어있습니다.", this);
            return;
        }

        if (toggleMarkerObject)
        {
            if (questMarker.gameObject.activeSelf != on)
                questMarker.gameObject.SetActive(on);
        }
        else
        {
            questMarker.enabled = on;
        }
    }

    // ───────────── 외부 지점 이동(옵션) ─────────────
    public IEnumerator GoTo(NavPoint p, bool reserve = false) => MoveToPoint(p, reserve);

    // ───────────── 이동 코어 ─────────────
    IEnumerator MoveToPoint(NavPoint p, bool reserve)
    {
        if (questMode) ShowMarker(false);   // 이동 시작하면 항상 끔
        UnlockFace();

        Vector3Int targetCell = grid.WorldToCell(p.transform.position);
        Vector3Int destCell = PickApproachCell(targetCell, p.GetOffsetsOrDefault(), reserve);

        Vector3Int startCell = grid.WorldToCell(foot.position);
        List<Vector3Int> path = AStarPathfinder.FindPath(startCell, destCell, IsBlocked, allowDiagonal, true);
        if (path == null || path.Count == 0) { ReleaseMyReservation(); yield break; }

        int idx = (path.Count > 0 && path[0] == startCell) ? 1 : 0;

        for (; idx < path.Count; idx++)
        {
            Vector3 targetWorld = CellCenterForFoot(path[idx]);

            Vector3Int fromCell = (idx > 0) ? path[idx - 1] : startCell;
            Vector3Int seg = path[idx] - fromCell;

            Vector2 segBlend = CellDeltaToBlend(seg);
            if (segBlend == Vector2.zero)
            {
                Vector3 fallbackDir = (targetWorld - foot.position);
                segBlend = WorldDirToBlend(fallbackDir);
            }

            SetBlend(segBlend);
            anim.SetFloat("Speed", 1f);

            while (Vector2.Distance(foot.position, targetWorld) > arriveThreshold)
            {
                Vector3 dir = (targetWorld - foot.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;
                yield return null;
            }
            yield return null;
        }

        anim.SetFloat("Speed", 0f);
        ApplyFace(p);
        ReleaseMyReservation();
    }

    Vector3Int PickApproachCell(Vector3Int targetCell, Vector3Int[] offsets, bool reserve)
    {
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

        var fallback = GetNearestReachableCell(targetCell + new Vector3Int(-1, 0, 0), 6);
        if (reserve && !CellReservation.IsReserved(fallback) && CellReservation.TryReserve(fallback, this))
            myReservedCell = fallback;
        return fallback;
    }

    Vector3 CellCenterForFoot(Vector3Int cell)
    {
        Vector3 center = grid.GetCellCenterWorld(cell);
        Vector3 footToRoot = transform.position - foot.position;
        return center + footToRoot;
    }

    // ───────────── ★ 길막 판정 (개선 버전) ─────────────
    bool IsBlocked(Vector3Int cell)
    {
        Vector2 p = grid.GetCellCenterWorld(cell);

        if (useBoxProbe)
        {
            // 셀 크기 기준 박스 탐지(타일/벽/선반에 안정적)
            Vector2 size = Vector2.Scale((Vector2)grid.cellSize, new Vector2(boxSizeScale, boxSizeScale));
            var hits = Physics2D.OverlapBoxAll(p, size, 0f, obstacleLayer);
            if (hits.Length == 0) return false;
            if (!ignoreTriggers) return true;

            foreach (var h in hits)
                if (!h.isTrigger) return true;   // 트리거가 아닌 실제 콜라이더만 길막으로 간주
            return false;
        }
        else
        {
            // 원형 프로브(기존 방식) + 트리거 무시 옵션
            var filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = obstacleLayer,
                useTriggers = !ignoreTriggers
            };
            Collider2D[] results = new Collider2D[8];
            int n = Physics2D.OverlapCircle(p, cellProbeRadius, filter, results);
            return n > 0;
        }
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

