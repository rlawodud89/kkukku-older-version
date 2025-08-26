using System.Collections;
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
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Employees = new Dictionary<int, (Employee employee, ProgressCircle progressCircle)>();
    }

    private void Start()
    {
        gameManager = GameManager.getInstance();
    }

    public void HandleMakeClicked(ItemScript currentYarn, BlanketSlotUI slotUI)
    {
        Employee current_employee = Employees[CurrentID].employee;
        
        if (current_employee.isWorking)
        {
            Debug.Log("작업자가 이미 다른 작업을 하고 있습니다!");
            return;
        }

        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;

        // 람다식에 사용할 로컬 변수 생성
        Employee employeeForLambda = current_employee;


        Debug.Log("Make_Cotton에서 Make 버튼 클릭됨 감지!");
        gameManager.Use_InventoryItem(currentYarn.itemName, 1);

        if (gameManager.Count_InventoryItem(currentYarn.itemName) > 0)
        {
            // 재료가 남아있으면 개수만 업데이트합니다.
            slotUI.SetData(currentYarn, gameManager.Count_InventoryItem(currentYarn.itemName));
        }
        else
        {
            // 재료가 없으면 슬롯을 비웁니다.
            slotUI.ClearSlot();
        }

        currentCotton = gameManager.Yarn_to_Cotton(currentYarn.itemName);
        current_employee.workItem = currentCotton;
        gameManager.Set_Worker_workingItem(current_employee.EmployeeID, currentCotton.itemName);

        cottonPanel.SetActive(false);
        current_employee.Working();

        progress_circle.CompleteCircle(current_employee.EmployeeID);
    }


    void showcotton(Employee employee)
    {
        ProgressCircle progress_circle = Employees[employee.EmployeeID].progressCircle;
        GameObject ballon_Panel = employee.ballonPanel;
        Button cotton_button = employee.ItemButton;

        if (employee.workItem != null)
        {
            ballon_Panel.SetActive(true);
            cotton_button.gameObject.SetActive(true);
            cotton_button.image.sprite = employee.workItem.image;

            cotton_button.onClick.RemoveAllListeners();
            cotton_button.onClick.AddListener(() =>
            {
                ballon_Panel.SetActive(false);
                cotton_button.gameObject.SetActive(false);
                progress_circle.ProgressInit();

                gameManager.Set_Worker_workingItem(employee.EmployeeID, null);
                gameManager.Add_InventoryItem(employee.workItem.itemName, 1);
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

        // Employee의 OnWorkComplete 이벤트에 showcotton 함수를 할당합니다.
        employee.OnWorkComplete = () => {
            showcotton(employee);
        };

        // 직원이 추가될 때 자신의 상태를 스스로 초기화하도록 합니다.
        employee.InitializeWorker();
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