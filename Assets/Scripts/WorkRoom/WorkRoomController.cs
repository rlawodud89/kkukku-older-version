using UnityEngine;

public class WorkRoomController : MonoBehaviour
{
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
        Employee[] employees = FindObjectsOfType<Employee>();
        foreach (var emp in employees)
        {
            if (emp.staminar.CompareTag(tag))
            {
                emp.staminar.RechargeFullStamina();
                Debug.Log($"[Upgrade Event] {emp.EmployeeName}({tag}) 풀충전 완료: {emp.staminar.currentStamina}/{emp.staminar.maxStamina}");
            }
        }
    }

}
