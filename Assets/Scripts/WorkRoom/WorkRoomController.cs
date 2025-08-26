using UnityEngine;

public class WorkRoomController : MonoBehaviour
{
    private GameManager gameManager;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        gameManager = GameManager.getInstance();
        gameManager.OnLevelUpgraded += OnUpgradeHandler;
        Debug.Log("[WorkRoomController] 이벤트 구독 완료");
    }

    void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnLevelUpgraded -= OnUpgradeHandler;
            Debug.Log("[WorkRoomController] 이벤트 해제");
        }
    }

    private void OnUpgradeHandler(string tag)
    {
        Debug.Log($"[WorkRoomController] OnUpgradeHandler 호출됨: 받은 태그={tag}");

        // 씬에 있는 모든 Employee 검색
        Employee[] employees = FindObjectsOfType<Employee>();

        foreach (var emp in employees)
        {
            // 이름으로 필터링: Employee1(Clone)
            if (emp.gameObject.name == "Employee1(Clone)")
            {
                float delta = emp.staminar.maxStamina - emp.staminar.currentStamina;
                if (delta > 0)
                {
                    emp.staminar.currentStamina = emp.staminar.maxStamina;
                    emp.staminar.StaminarUI();

                    // DB에도 반영
                    gameManager.Change_Worker_Stamina(emp.EmployeeID, (int)delta);

                    Debug.Log($"[WorkRoomController] {emp.EmployeeName} 스태미너 +{delta} → 풀충전 완료!");
                }
                else
                {
                    Debug.Log($"[WorkRoomController] {emp.EmployeeName} 이미 풀스태미나 상태.");
                }
            }
        }
    }
}
