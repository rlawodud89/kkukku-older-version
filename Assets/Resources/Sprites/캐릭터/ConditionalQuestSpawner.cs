using UnityEngine;

public class ConditionalQuestSpawner : MonoBehaviour
{
    // ── 0) 외부 트리거 ───────────────────────────────────────────
    [Header("Trigger")]
    [Tooltip("외부 스크립트/인스펙터에서 true로 바꾸면 즉시 1회 스폰합니다.")]
    public bool triggerSpawn = false;   // ← true로 바꾸면 1회 스폰
    public bool oneShot = true;         // true면 한 번만 스폰
    bool hasSpawned = false;

    // ── 1) 프리팹/위치 ───────────────────────────────────────────
    [Header("Prefab & Spawn")]
    public GameObject prefab;           // 특정 캐릭터 프리팹(재사용 안함)
    public Transform spawnPoint;        // 비우면 자기 Transform

    // ── 2) AStarMover 주입 ───────────────────────────────────────
    [Header("A* / Route injection")]
    public Grid grid;
    public NavPoint startPoint;
    public NavPoint[] doorPoints;

    // ── 3) 퀘스트 모드 세팅(AStarMover가 처리) ───────────────────
    [Header("Quest (AStarMover)")]
    public NavPoint questWaitPoint;
    public float questWaitSeconds = 10f;

    // (선택) 프리팹 자식 Canvas를 이름으로 찾고 싶으면 지정
    [Header("Optional")]
    public string questCanvasChildName = "QuestCanvas";

    void Awake()
    {
        if (!spawnPoint) spawnPoint = transform;
    }

    void Start()
    {
        // 시작 시 이미 true면 즉시 1회 스폰
        TrySpawnIfTriggered();
    }

    void Update()
    {
        // 런타임에 true로 바뀌면 즉시 1회 스폰
        TrySpawnIfTriggered();
    }

    void TrySpawnIfTriggered()
    {
        if (hasSpawned && oneShot) return;

        if (triggerSpawn)
        {
            triggerSpawn = false;   // 연타 방지
            SpawnNow();

            if (oneShot)
            {
                hasSpawned = true;
                enabled = false;    // 더 이상 체크 안 함
            }
        }
    }

    void SpawnNow()
    {
        if (!prefab) { Debug.LogWarning("[ConditionalQuestSpawner] Prefab이 비어있음"); return; }

        var obj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        // AStarMover 확보
        var mover = obj.GetComponent<AStarMover>();
        if (!mover) mover = obj.AddComponent<AStarMover>();

        // 기본 경로 의존성 주입
        mover.Init(grid, startPoint, doorPoints, null, null);

        // 퀘스트 모드 on
        mover.questMode = true;
        mover.questWaitPoint = questWaitPoint;
        mover.questWaitSeconds = questWaitSeconds;

        // ── 자식 Canvas 자동 연결 ──────────────────────────────
        if (!mover.questCanvas)
        {
            Canvas qc = null;

            // 1) 이름으로 우선 탐색
            if (!string.IsNullOrEmpty(questCanvasChildName))
            {
                var t = obj.transform.Find(questCanvasChildName);
                if (t) qc = t.GetComponent<Canvas>();
            }

            // 2) 실패하면 첫 번째 자식 Canvas 사용
            if (!qc) qc = obj.GetComponentInChildren<Canvas>(true);

            mover.questCanvas = qc;
        }

        // 스폰 시점엔 항상 꺼두기 (AStarMover가 멈출 때만 켬)
        if (mover.questCanvas)
        {
            if (mover.toggleCanvasObject)
                mover.questCanvas.gameObject.SetActive(false);
            else
                mover.questCanvas.enabled = false;
        }
        else
        {
            Debug.LogWarning("[ConditionalQuestSpawner] 자식 Canvas를 찾지 못해 AStarMover.questCanvas를 설정하지 못했습니다.", obj);
        }
    }

    // (옵션) 코드로 호출하고 싶을 때: spawner.Activate();
    public void Activate() => triggerSpawn = true;
}
