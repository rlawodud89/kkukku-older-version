using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Make_Sewing : MonoBehaviour
{
    public static Make_Sewing Instance { get; private set; }

    public GameObject sewingPanel;

    private Dictionary<int, (Employee employee, ProgressCircle progressCircle)> Employees;
    private int CurrentID;

    public GameObject BallonPanel;
    public GameObject CompletePanel;

    public Button SewingButton;
    public Image CompleteImage;
    public TextMeshProUGUI CompleteText;

    private GameManager gameManager;
    private ItemScript currentBlanket;
    public bool isMaking;

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
                    showsewing();
                };

                progressCircle.CompleteCircle(employee.EmployeeID, employee.workingPercent);
            }
            else if (employee.workItem != null)
            {
                showsewing();
            }
        }
    }

  

    public void HandleMakeClicked(ItemScript currentSewing)
    {
        Employee current_employee = Employees[CurrentID].employee;
        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;

        currentBlanket = gameManager.Cotton_to_Blanket(currentSewing.itemName);
        current_employee.workItem = currentBlanket;
        gameManager.Set_Worker_workingItem(current_employee.EmployeeID, currentBlanket.itemName);

        Debug.Log("Make_Sewing에서 Make 버튼 클릭됨 감지!");
        gameManager.Use_InventoryItem(currentBlanket.cottonName, 1);

        isMaking = true;
        sewingPanel.SetActive(false);
        current_employee.Working();

        progress_circle.OnComplete = () =>
        {

            gameManager.Set_Worker_WorkingPercent(current_employee.EmployeeID, 0f);
            Debug.Log("완성");
            showsewing();
        };

        progress_circle.CompleteCircle(current_employee.EmployeeID);

    }


    void showsewing()
    {
        Employee current_employee = Employees[CurrentID].employee;
        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;
        GameObject ballon_Panel = current_employee.ballonPanel;
        Button sewing_button = current_employee.ItemButton;

        if (current_employee.workItem != null)
        {
            ballon_Panel.SetActive(true);
            sewing_button.gameObject.SetActive(true);
            sewing_button.image.sprite = current_employee.workItem.image;

            sewing_button.onClick.RemoveAllListeners();
            sewing_button.onClick.AddListener(() =>
            {

                ballon_Panel.SetActive(false);
                sewing_button.gameObject.SetActive(false);
                progress_circle.ProgressInit();

                CompletePanel.SetActive(true);
                CompleteImage.sprite = current_employee.workItem.image;
                CompleteText.text = current_employee.workItem.itemName + "이 완성되었습니다!";
                isMaking = false;

                gameManager.Set_Worker_workingItem(current_employee.EmployeeID, null);
                gameManager.Add_InventoryItem(current_employee.workItem.itemName, 1); //원단 추가

            });

        }
        else
        {
            Debug.Log("null");
        }
    }

    public void ClickCompleteBtn()
    {
        CompletePanel.SetActive(false);
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
