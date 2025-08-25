using UnityEngine;

public class WorkRoomController : MonoBehaviour
{
    public Employee employee;
    private GameManager gameManager;

    void OnEnable()
    {
        UpgradeShopController1.OnUpgrade += OnUpgradeHandler;
    }

    void OnDisable()
    {
        UpgradeShopController1.OnUpgrade -= OnUpgradeHandler;
    }

    private void OnUpgradeHandler(string tag)
    {

        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        if (gameObject.tag == tag)
        {
            float delta = employee.staminar.maxStamina - employee.staminar.currentStamina;
            if (delta > 0)
            {
                gameManager.Change_Worker_Stamina(employee.EmployeeID, (int)delta);
                employee.staminar.currentStamina = employee.staminar.maxStamina;
                employee.staminar.StaminarUI();
                Debug.Log($"[WorkRoom] {employee.EmployeeName}({tag}) 스태미나 +{delta} 풀충전 완료!");
            }
        }
    }


}
