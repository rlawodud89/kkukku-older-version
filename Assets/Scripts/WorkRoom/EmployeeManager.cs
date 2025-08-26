using UnityEngine;
using System.Linq;

public class EmployeeManager : MonoBehaviour
{

    private GameManager gameManager;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        if (gameManager.isCatUpgraded || gameManager.isFoxUpgraded || gameManager.isSheepUpgraded)
        {
            FindAndProcessAllEmployees();
        }
  }

    // 이 함수를 호출하면 씬의 모든 "Employee1(Clone)"을 찾습니다.
    public void FindAndProcessAllEmployees()
    {
        Employee[] allEmployees = FindObjectsOfType<Employee>();

        foreach (Employee emp in allEmployees)
        {
            // 먼저 maxStamina 값을 업데이트합니다.
            emp.staminar.UpdateMaxStamina();

            // 이제 업데이트된 maxStamina를 사용하여 스태미나를 채웁니다.
            if (emp.gameObject.name == "Employee1(Clone)" && gameManager.isFoxUpgraded)
            {
                int delta = (int)(emp.staminar.maxStamina - emp.staminar.currentStamina);

                emp.staminar.currentStamina = emp.staminar.maxStamina;
                gameManager.Change_Worker_Stamina(emp.EmployeeID, delta);
            }
            else if (emp.gameObject.name == "Employee2(Clone)" && gameManager.isSheepUpgraded)
            {
                int delta = (int)(emp.staminar.maxStamina - emp.staminar.currentStamina);

                emp.staminar.currentStamina = emp.staminar.maxStamina;
                gameManager.Change_Worker_Stamina(emp.EmployeeID, delta);
            }
            else if (emp.gameObject.name == "Employee3(Clone)" && gameManager.isCatUpgraded)
            {
                int delta = (int)(emp.staminar.maxStamina - emp.staminar.currentStamina);

                emp.staminar.currentStamina = emp.staminar.maxStamina;
                gameManager.Change_Worker_Stamina(emp.EmployeeID, delta);
            }
        }

        gameManager.isFoxUpgraded = false;
        gameManager.isSheepUpgraded = false;
        gameManager.isCatUpgraded = false;
    }
}