using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Make_Cotton : MonoBehaviour
{
    public static Make_Cotton Instance { get; private set; }

    public GameObject cottonPanel;
    public SewingPanel sewingPanel;

    private Dictionary<int, (Employee employee, ProgressCircle progressCircle)> Employees;
    private int CurrentID;

    public GameObject BallonPanel;
    public Button CottonButton;

    private ItemScript currentYarn;
    private ItemScript currentCotton;

    private GameManager gameManager;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }

        Instance = this;

        Employees = new Dictionary<int, (Employee employee, ProgressCircle progressCircle)>();
    }

    private void Start()
    {
        gameManager = GameManager.getInstance();

        foreach (var e in Employees)
        {
            Employee employee = e.Value.employee;
            ProgressCircle progressCircle = e.Value.progressCircle;
            CurrentID = employee.EmployeeID;

            if (employee.workingPercent != 0f)
            {

                progressCircle.OnComplete = () =>
                {
                    gameManager.Set_Worker_WorkingPercent(employee.EmployeeID, 0f);
                    showcotton();
                };

                progressCircle.CompleteCircle(employee.EmployeeID, employee.workingPercent);
            }
            else if (employee.workItem != null)
            {
                showcotton();
            }
        }
    }



    public void HandleMakeClicked(ItemScript currentYarn)
    {
        Employee current_employee = Employees[CurrentID].employee;
        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;

        Debug.Log("Make_Cotton에서 Make 버튼 클릭됨 감지!");
        gameManager.Use_InventoryItem(currentYarn.itemName, 1);

        currentCotton = gameManager.Yarn_to_Cotton(currentYarn.itemName);
        current_employee.workItem = currentCotton;
        gameManager.Set_Worker_workingItem(current_employee.EmployeeID, currentCotton.itemName);

        cottonPanel.SetActive(false);
        current_employee.Working();

        progress_circle.OnComplete = () =>
        {
            gameManager.Set_Worker_WorkingPercent(current_employee.EmployeeID, 0f);
            showcotton();
        };

        progress_circle.CompleteCircle(current_employee.EmployeeID);
    }


    void showcotton()
    {
        Employee current_employee = Employees[CurrentID].employee;
        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;
        GameObject ballon_Panel = current_employee.ballonPanel;
        Button cotton_button = current_employee.ItemButton;

        if (current_employee.workItem != null)
        {
            ballon_Panel.SetActive(true);
            cotton_button.gameObject.SetActive(true);
            cotton_button.image.sprite = current_employee.workItem.image;

            cotton_button.onClick.RemoveAllListeners();
            cotton_button.onClick.AddListener(() =>
            {
                ballon_Panel.SetActive(false);
                cotton_button.gameObject.SetActive(false);
                progress_circle.ProgressInit();

                gameManager.Set_Worker_workingItem(current_employee.EmployeeID, null);
                gameManager.Add_InventoryItem(current_employee.workItem.itemName, 1); //원단 추가

                sewingPanel?.SetSelectedBlanket();

            });

        }
        else
        {
            Debug.Log("null");
        }
    }

    public void Add_Employee(Employee employee, ProgressCircle progressCircle)
    {
        Employees.Add(employee.EmployeeID, (employee, progressCircle));
    }

    public void Remove_Employee(int employeeID)
    {
        Employees.Remove(employeeID);
    }

    public void Set_CurrentEmployee(int employeeID)
    {
        CurrentID = employeeID;
    }
}
