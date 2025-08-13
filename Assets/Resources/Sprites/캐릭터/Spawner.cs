using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    // ─────────── 1. 스폰 대상 프리팹 ───────────
    [Header("무작위로 뽑아 쓸 캐릭터 프리팹들")]
    public GameObject[] prefabs;

    [Header("스폰 위치 (없으면 자기 Transform)")]
    public Transform spawnPoint;

    // ─────────── 2. AStarMover 주입용 레퍼런스 ─────────── ★
    [Header("AStarMover 경로 의존성(선택)")]
    public Grid grid;                       // Isometric Grid
    public NavPoint startPoint;
    public NavPoint[] doorPoints;
    public NavPoint[] shelfPoints;
    public NavPoint cashierPoint;

    // ─────────── 3. 트리거용 UI 버턴 ───────────
    [Header("UI Button (없으면 자동 시작)")]
    public Button spawnButton;

    // ─────────── 4. 스폰 간격 ───────────
    [Header("스폰 간격 [초] (Min~Max)")]
    public Vector2 intervalRange = new Vector2(0f, 3f);

    Coroutine loop;   // 이미 돌고 있는지 체크

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

    /// <summary>스폰 루프 시작(이미 돌고 있으면 무시)</summary>
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

        // ② AStarMover 컴포넌트가 있으면 Init 주입 ──────── ★
        var mover = obj.GetComponent<AStarMover>();
        if (mover != null)
        {
            // Grid / NavPoint 값이 할당돼 있지 않다면
            // mover 내부에서 FindObjectOfType 으로 fallback 하므로
            // 여기서는 null이어도 그대로 넘겨도 OK
            mover.Init(grid,              // null이면 Awake에서 자동 Find
                       startPoint,
                       doorPoints,
                       shelfPoints,
                       cashierPoint);
        }
    }
}
