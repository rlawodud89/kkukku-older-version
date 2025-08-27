using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Make_Cotton : MonoBehaviour
{
    public static Make_Cotton Instance { get; private set; }

    public GameObject cottonPanel;
    public SewingPanel sewingPanel;
    public CottonPanel cotton_panel;
    public TextMeshProUGUI announce_text;

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
            ShowAnnounceText("이미 작업 중입니다.", 2f);
            return;
        }

        if (current_employee.lackStamina())
        {
            Debug.Log("스태미너가 부족합니다!");
            ShowAnnounceText("스태미너가 부족합니다.", 2f);
            return;
        }

        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;

        // 람다식에 사용할 로컬 변수 생성
        Employee employeeForLambda = current_employee;


        gameManager.Use_InventoryItem(currentYarn.itemName, 1);

        cotton_panel.RefreshInventoryUI();

        currentCotton = gameManager.Yarn_to_Cotton(currentYarn.itemName);
        current_employee.workItem = currentCotton;
        gameManager.Set_Worker_workingItem(current_employee.EmployeeID, currentCotton.itemName);

        cottonPanel.SetActive(false);
        current_employee.Working();

        progress_circle.CompleteCircle(current_employee.EmployeeID);
    }


    public void showcotton(Employee employee)
    {
        ProgressCircle progress_circle = Employees[employee.EmployeeID].progressCircle;
        GameObject ballon_Panel = employee.ballonPanel;
        Button cotton_button = employee.ItemButton;

        if (employee.workItem != null)
        {
            ballon_Panel.SetActive(true);
            cotton_button.gameObject.SetActive(true);
            cotton_button.image.sprite = employee.workItem.image;

            // 안전하게 로컬 변수에 저장
            ItemScript finishedItem = employee.workItem;

            cotton_button.onClick.RemoveAllListeners();
            cotton_button.onClick.AddListener(() =>
            {
                ballon_Panel.SetActive(false);
                cotton_button.gameObject.SetActive(false);
                progress_circle.ProgressInit();

                // 진행도 초기화
                if (employee.workingPercent > 0)
                {
                    gameManager.Set_Worker_WorkingPercent(employee.EmployeeID, 0);
                    employee.workingPercent = 0;
                }

                // 작업 완료 처리
                gameManager.Set_Worker_workingItem(employee.EmployeeID, null);
                gameManager.Add_InventoryItem(finishedItem.itemName, 1);

                employee.workItem = null;
                progress_circle.elapsed = 0f;
                employee.isWorking = false;

                sewingPanel?.SetSelectedBlanket();
            });
        }
        else
        {
            Debug.Log("null");
        }
    }

    private void ShowAnnounceText(string text, float duration)
    {
        if (announce_text == null) return;

        announce_text.text = text;
        announce_text.gameObject.SetActive(true);
        StartCoroutine(HideAnnounceTextAfterDelay(duration));
    }

    private IEnumerator HideAnnounceTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        announce_text.gameObject.SetActive(false);
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