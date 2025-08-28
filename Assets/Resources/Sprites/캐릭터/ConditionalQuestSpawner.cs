using System.Collections;
using UnityEngine;

public class ConditionalQuestSpawner : MonoBehaviour
{
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
    public string questMarkerChildName = "QuestMarker";

    private GameManager gameManager;
    private bool hasSpawned = false;

    private const string SpawnedKey = "ConditionalQuestSpawner_Spawned";

    private IEnumerator Start()
    {
        if (PlayerPrefs.GetInt(SpawnedKey, 0) == 1)
        {
            hasSpawned = true;
            enabled = false;
            yield break;
        }

        while (gameManager == null)
        {
            gameManager = GameManager.getInstance();
            yield return null;
        }

        TrySpawnByEnergy();
    }

    private void Awake()
    {
        if (!spawnPoint) spawnPoint = transform;
    }

    private void Update()
    {
        TrySpawnByEnergy();
    }

    private void TrySpawnByEnergy()
    {
        if (hasSpawned) return;
        if (gameManager == null) return;

        if (gameManager.Get_EnergyLevel() == 2)
        {
            SpawnNow();
            hasSpawned = true;
            PlayerPrefs.SetInt(SpawnedKey, 1);
            PlayerPrefs.Save();
            enabled = false;
        }
    }

    private void SpawnNow()
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

    // 🔹 인스펙터에서 우클릭 → Reset Spawn Key 실행 가능
    [ContextMenu("Reset Spawn Key (PlayerPrefs)")]
    private void ResetSpawnKey()
    {
        PlayerPrefs.DeleteKey(SpawnedKey);
        PlayerPrefs.Save();
        Debug.Log($"[ConditionalQuestSpawner] Spawn 키 '{SpawnedKey}' 삭제됨");
    }
}