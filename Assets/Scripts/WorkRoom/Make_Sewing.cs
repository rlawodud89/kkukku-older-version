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

    public void HandleMakeClicked(ItemScript currentSewing, BlanketSlotUI slotUI)
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


        currentBlanket = gameManager.Cotton_to_Blanket(currentSewing.itemName);
        current_employee.workItem = currentBlanket;
        gameManager.Set_Worker_workingItem(current_employee.EmployeeID, currentBlanket.itemName);



        if (gameManager.Count_InventoryItem(currentSewing.itemName) > 0)
        {
            // 재료가 남아있으면 개수만 업데이트합니다.
            slotUI.SetData(currentSewing, gameManager.Count_InventoryItem(currentSewing.itemName));
        }
        else
        {
            // 재료가 없으면 슬롯을 비웁니다.
            slotUI.ClearSlot();
        }

        isMaking = true;
        sewingPanel.SetActive(false);
        current_employee.Working();

        progress_circle.CompleteCircle(current_employee.EmployeeID);
    }

    // showsewing 함수가 Employee 객체를 인수로 받도록 수정
    void showsewing(Employee employee)
    {
        ProgressCircle progress_circle = Employees[employee.EmployeeID].progressCircle;
        GameObject ballon_Panel = employee.ballonPanel;
        Button sewing_button = employee.ItemButton;

        if (employee.workItem != null)
        {
            ballon_Panel.SetActive(true);
            sewing_button.gameObject.SetActive(true);
            sewing_button.image.sprite = employee.workItem.image;

            sewing_button.onClick.RemoveAllListeners();
            sewing_button.onClick.AddListener(() =>
            {
                ballon_Panel.SetActive(false);
                sewing_button.gameObject.SetActive(false);
                progress_circle.ProgressInit();

                CompletePanel.SetActive(true);
                CompleteImage.sprite = employee.workItem.image;
                CompleteText.text = employee.workItem.itemName + "이 완성되었습니다!";
                isMaking = false;

                gameManager.Set_Worker_workingItem(employee.EmployeeID, null);
                gameManager.Add_InventoryItem(employee.workItem.itemName, 1);
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

        employee.OnWorkComplete = () => {
            showsewing(employee);
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