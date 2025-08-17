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

    // ── 4) 마커 스프라이트 연결 ──────────────────────────────────
    [Header("Optional")]
    [Tooltip("프리팹 자식 중 퀘스트 마커(SpriteRenderer)가 붙은 오브젝트 이름")]
    public string questMarkerChildName = "QuestMarker";

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
        if (!prefab)
        {
            Debug.LogWarning("[ConditionalQuestSpawner] Prefab이 비어있음");
            return;
        }

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

        // ── 자식 SpriteRenderer 마커 자동 연결 ──────────────────
        if (!mover.questMarker)
        {
            if (!string.IsNullOrEmpty(questMarkerChildName))
            {
                var t = obj.transform.Find(questMarkerChildName);
                if (t) mover.questMarker = t.GetComponent<SpriteRenderer>();
            }
        }

        // 스폰 시점엔 항상 OFF (AStarMover가 멈출 때만 ON)
        if (mover.questMarker)
        {
            if (mover.toggleMarkerObject) mover.questMarker.gameObject.SetActive(false);
            else mover.questMarker.enabled = false;
        }
        else
        {
            Debug.LogWarning("[ConditionalQuestSpawner] 자식 마커(SpriteRenderer)를 찾지 못했습니다. 자식 이름을 '" + questMarkerChildName + "'로 두고 SpriteRenderer를 붙이세요.", obj);
        }
    }

    // (옵션) 코드로 호출하고 싶을 때: spawner.Activate();
    public void Activate() => triggerSpawn = true;
}
