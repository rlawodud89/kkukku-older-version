using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class Make_Fabric : MonoBehaviour
{
    public static Make_Fabric Instance { get; private set; }

    public GameObject Panel;
    public GameObject Panel2;
    public GameObject Scroll_View;

    private Dictionary<int, (Employee employee, ProgressCircle progressCircle)> Employees;
    private int CurrentID;

    public ItemScript currentBlanket;
    public ItemScript currentYarn;
    public CottonPanel cottonPanel;
    public FabricDetailPanelController detailPanelController;

    private GameManager gameManager;
    private bool can_make = false;

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

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        if (detailPanelController == null)
        {
            detailPanelController = FindObjectOfType<FabricDetailPanelController>();
        }
    }

    public void ClickMakebtn()
    {
        // 딕셔너리에서 현재 직원을 가져옵니다.
        Employee current_employee = Employees[CurrentID].employee;
        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;

        // 람다식에 사용할 로컬 변수 생성
        Employee employeeForLambda = current_employee;

        if (current_employee.isWorking)
        {
            Debug.Log("작업자가 이미 바쁩니다!");
            can_make = false; // 작업 중이므로 제작 불가
        }
        else
        {
            // 작업자가 놀고 있다면, 레시피를 확인합니다.
            can_make = Check_Recipe(currentBlanket);
        }

        if (can_make)
        {
            for (int i = 0; i < currentBlanket.recipe.Count; i++)
            {
                gameManager.Use_InventoryItem(currentBlanket.recipe[i].itemName, currentBlanket.recipe[i].count);
                Debug.Log(currentBlanket.recipe[i].itemName + currentBlanket.recipe[i].count + "만큼 감소");
            }
            currentYarn = gameManager.Blanket_to_Yarn(currentBlanket.itemName);
            current_employee.workItem = currentYarn;
            gameManager.Set_Worker_workingItem(current_employee.EmployeeID, currentYarn.itemName);

            if (detailPanelController == null)
            {
                detailPanelController = FindObjectOfType<FabricDetailPanelController>();
            }
            detailPanelController.OpenPanel(currentBlanket);

            Panel.SetActive(false);
            Panel2.SetActive(false);
            Scroll_View.SetActive(false);

            current_employee.Working();

            // 람다식에 로컬 변수를 사용하여 클로저 버그 방지
            progress_circle.OnComplete = () =>
            {
                gameManager.Set_Worker_WorkingPercent(employeeForLambda.EmployeeID, 0f);
                Debug.Log(currentBlanket.yarnName + "만듦");
                showfabric(employeeForLambda);
            };

            progress_circle.CompleteCircle(current_employee.EmployeeID);
            can_make = false;
        }
        else
        {
            Debug.Log("제작할 수 없습니다!");
        }
    }

    private bool Check_Recipe(ItemScript currentBlanket)
    {
        List<(ItemScript data, int count)> inv = gameManager.Get_Material_Inventory();

        // recipe에 있는 재료 하나씩 확인
        foreach (var recipeItem in currentBlanket.recipe)
        {
            var invItem = inv.Find(x => x.data.itemName == recipeItem.itemName);

            // 없거나 개수가 부족하면 false
            if (invItem.data == null || invItem.count < recipeItem.count)
            {
                return false;
            }
        }

        return true; // 모든 재료 충분
    }


    public void showfabric(Employee employee)
    {
        GameObject ballon_Panel = employee.ballonPanel;
        Button fabric_button = employee.ItemButton;
        ProgressCircle progress_circle = Employees[employee.EmployeeID].progressCircle;

        if (employee.workItem != null)
        {
            ballon_Panel.SetActive(true);
            fabric_button.gameObject.SetActive(true);
            fabric_button.image.sprite = employee.workItem.image;

            fabric_button.onClick.RemoveAllListeners();
            fabric_button.onClick.AddListener(() =>
            {
                ballon_Panel.SetActive(false);
                fabric_button.gameObject.SetActive(false);
                progress_circle.ProgressInit();

                gameManager.Set_Worker_workingItem(employee.EmployeeID, null);
                gameManager.Add_InventoryItem(employee.workItem.itemName, 1);

                cottonPanel?.SetSelectedBlanket();
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

        employee.OnWorkComplete = () => {
            showfabric(employee);
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
