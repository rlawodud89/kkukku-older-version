using System.Collections;
using UnityEngine;

public class ConditionalQuestSpawner : MonoBehaviour
{
    [Header("Trigger")]
    [Tooltip("외부 스크립트/인스펙터에서 true로 바꾸면 즉시 1회 스폰합니다.triggerSpawn은 디버깅용임")]
    public bool triggerSpawn = false;
    public bool oneShot = true;
    bool hasSpawned = false;

    [Header("Prefab & Spawn")]
    public GameObject prefab;
    public Transform spawnPoint;

    [Header("A* / Route injection")]
    public Grid grid;
    public NavPoint startPoint;
    public NavPoint[] doorPoints;

    [Header("Quest (AStarMover)")]
    public NavPoint questWaitPoint;
    public float questWaitSeconds = 10f;

    [Header("Optional")]
    [Tooltip("프리팹 자식 중 퀘스트 마커(SpriteRenderer)가 붙은 오브젝트 이름")]
    public string questMarkerChildName = "QuestMarker";

    private GameManager gameManager;

    private const string SpawnedKey = "ConditionalQuestSpawner_Spawned";

    private IEnumerator Start()
    {
        // 이미 소환된 기록이 있으면 아예 비활성화
        if (PlayerPrefs.GetInt(SpawnedKey, 0) == 1)
        {
            hasSpawned = true;
            if (oneShot) enabled = false;
            yield break;
        }

        // GameManager 인스턴스가 유효할 때까지 대기
        while (gameManager == null)
        {
            gameManager = GameManager.getInstance();
            yield return null; // 한 프레임 대기
        }

        TrySpawnIfTriggered();
    }

    void Awake()
    {
        if (!spawnPoint) spawnPoint = transform;
    }

    void Update()
    {
        TrySpawnIfTriggered();
    }

    void TrySpawnIfTriggered()
    {
        if (hasSpawned && oneShot) return;

        // 1) 디버그용 트리거 (저장 안 함)
        if (triggerSpawn)
        {
            triggerSpawn = false;
            SpawnNow();
            if (oneShot) hasSpawned = true;
            return;
        }

        // 2) GameManager 에너지 조건 (저장)
        if (!hasSpawned && gameManager != null && gameManager.Get_EnergyLevel() == 2)
        {
            SpawnNow();
            MarkAsSpawned(); // 이때만 저장
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

        var mover = obj.GetComponent<AStarMover>();
        if (!mover) mover = obj.AddComponent<AStarMover>();

        mover.Init(grid, startPoint, doorPoints, null, null);

        mover.questMode = true;
        mover.questWaitPoint = questWaitPoint;
        mover.questWaitSeconds = questWaitSeconds;

        if (!mover.questMarker && !string.IsNullOrEmpty(questMarkerChildName))
        {
            var t = obj.transform.Find(questMarkerChildName);
            if (t) mover.questMarker = t.GetComponent<SpriteRenderer>();
        }

        if (mover.questMarker)
        {
            if (mover.toggleMarkerObject) mover.questMarker.gameObject.SetActive(false);
            else mover.questMarker.enabled = false;
        }
        else
        {
            Debug.LogWarning($"[ConditionalQuestSpawner] 자식 마커(SpriteRenderer)를 찾지 못했습니다. 자식 이름을 '{questMarkerChildName}'로 두고 SpriteRenderer를 붙이세요.", obj);
        }
    }

    void MarkAsSpawned()
    {
        hasSpawned = true;
        PlayerPrefs.SetInt(SpawnedKey, 1);
        PlayerPrefs.Save();
        if (oneShot) enabled = false;
    }

    public void Activate() => triggerSpawn = true;
}
