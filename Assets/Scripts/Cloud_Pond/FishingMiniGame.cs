using UnityEngine;
using UnityEngine.UI;

public class FishingMiniGame : MonoBehaviour
{
    public GameManager gameManager;
    public MaterialsInventory materialsInventory;

    public RectTransform pointer;     // 빨간 포인터
    public RectTransform successZone; // 초록 성공 구간
    public float speed = 200f;        // 포인터 이동 속도(px/s)
    public GameObject gamePanel;

    private ItemScript currentdata;
    private bool goingRight = true;
    public bool miniGameRunning = false;

    void Update()
    {
        if (!miniGameRunning) return;
        // 포인터 좌우 왕복
        float move = speed * Time.deltaTime * (goingRight ? 1 : -1);
        pointer.anchoredPosition += new Vector2(move, 0);

        // 경계 체크 (왼쪽 -200, 오른쪽 200 기준)
        if (pointer.anchoredPosition.x >= 160) goingRight = false;
        if (pointer.anchoredPosition.x <= -160) goingRight = true;

        // Space 키로 성공 여부 판단
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(successZone, pointer.position))
            {
                Debug.Log("🎯 성공! 재료 획득");
                FinishMiniGame(true);
            }
            else
            {
                Debug.Log("❌ 실패! 재료를 놓쳤습니다.");
                FinishMiniGame(false);
            }
        }
    }

    public void GetMaterial()
    {
        if (miniGameRunning) return;

        if (gameManager == null)
            gameManager = GameManager.getInstance();

        StartMiniGame();
    }

    private void StartMiniGame()
    {
        gamePanel.SetActive(true);
        pointer.anchoredPosition = new Vector2(-160, 5); // 시작 위치
        goingRight = true;
        miniGameRunning = true;
        // 성공 구간 위치 랜덤 (-100~100 범위)
        successZone.anchoredPosition = new Vector2(Random.Range(-100f, 100f), 5);

        // 성공 구간 크기 랜덤 (가로만 변경, 세로는 그대로)
        float randomWidth = Random.Range(40f, 100f); // 최소 40px ~ 최대 120px
        successZone.sizeDelta = new Vector2(randomWidth, successZone.sizeDelta.y);
        Debug.Log("🎣 미니게임 시작!");
    }

    private void FinishMiniGame(bool success)
    {
        miniGameRunning = false;

        if (success)
        {
            currentdata = gameManager.Get_Random_Material();
            materialsInventory.AddMaterial(currentdata);
            gameManager.Add_InventoryItem(currentdata.itemName, 1);
            Debug.Log($"{currentdata.itemName} 획득!");
        }
    }

    public void EndGame()
    {
        if (!miniGameRunning)
        {
            pointer.anchoredPosition = new Vector2(-160, 5); // 시작 위치
            gamePanel.SetActive(false);
        }
    }
}
