using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    // ─────────── 1) 스폰 대상 프리팹 ───────────
    [Header("무작위로 뽑아 쓸 캐릭터 프리팹들")]
    public GameObject[] prefabs;

    [Header("스폰 위치 (없으면 자기 Transform)")]
    public Transform spawnPoint;

    // ─────────── 2) AStarMover 경로 의존성 ───────────
    [Header("AStarMover 경로 의존성(선택)")]
    public Grid grid;                       // Isometric Grid
    public NavPoint startPoint;
    public NavPoint[] doorPoints;
    public NavPoint[] shelfPoints;
    public NavPoint cashierPoint;

    // ─────────── 3) AStarMover 인스펙터 오버라이드 ───────────
    [System.Serializable]
    public class MoverOverrides
    {
        [Tooltip("체크 시 아래 값들로 프리팹 기본값을 덮어씁니다.")]
        public bool enabled = true;

        [Header("Move")]
        public float moveSpeed = 2f;
        public bool allowDiagonal = false;
        public float arriveThreshold = 0.05f;
        public float waitAtShelfSeconds = 1.0f;
        public float waitAtCashierSeconds = 1.0f;

        [Header("Visits")]
        public int minShelfVisits = 2;
        public int maxShelfVisits = 5;

        [Header("Shop / Payment")]
        [Range(0f, 1f)]
        public float energyPerGold = 0.02f;

        [Header("Quest Mode")]
        public bool questMode = false;
        public bool toggleCanvasObject = true;
    }

    [Header("AStarMover 옵션(프리팹 오버라이드)")]
    public MoverOverrides mover = new MoverOverrides();

    // ─────────── 4) 트리거용 UI 버튼 ───────────
    [Header("UI Button (없으면 자동 시작)")]
    public Button spawnButton;

    // ─────────── 5) 스폰 간격 ───────────
    [Header("스폰 간격 [초] (Min~Max)")]
    public Vector2 intervalRange = new Vector2(0f, 3f);

    Coroutine loop;

    void Awake()
    {
        if (spawnButton != null)
            spawnButton.onClick.AddListener(StartSpawning);
    }

    void Start()
    {
        if (spawnButton == null)   // 버튼이 없으면 자동 시작
            StartSpawning();
    }

    public void StartSpawning()
    {
        if (loop == null)
            loop = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();
            float wait = Random.Range(intervalRange.x, intervalRange.y);
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnOne()
    {
        if (prefabs == null || prefabs.Length == 0) return;

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        Transform p = spawnPoint ? spawnPoint : transform;

        // ① 인스턴스 생성
        GameObject obj = Instantiate(prefab, p.position, p.rotation);

        // ② AStarMover 주입 및 옵션 오버라이드
        var moverComp = obj.GetComponent<AStarMover>();
        if (moverComp != null)
        {
            // 경로 의존성 주입 (비어있으면 AStarMover.Awake에서 자동 fallback)
            moverComp.Init(grid, startPoint, doorPoints, shelfPoints, cashierPoint);

            // 프리팹 기본값 덮어쓰기
            if (mover != null && mover.enabled)
            {
                moverComp.moveSpeed = mover.moveSpeed;
                moverComp.allowDiagonal = mover.allowDiagonal;
                moverComp.arriveThreshold = mover.arriveThreshold;
                moverComp.waitAtShelfSeconds = mover.waitAtShelfSeconds;
                moverComp.waitAtCashierSeconds = mover.waitAtCashierSeconds;

                moverComp.minShelfVisits = mover.minShelfVisits;
                moverComp.maxShelfVisits = mover.maxShelfVisits;

                moverComp.energyPerGold = mover.energyPerGold;

                moverComp.questMode = mover.questMode;
                moverComp.toggleCanvasObject = mover.toggleCanvasObject;
            }
        }
    }
}
