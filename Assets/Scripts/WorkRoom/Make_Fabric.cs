using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Make_Fabric : MonoBehaviour
{
    public static Make_Fabric Instance { get; private set; }

    public GameObject Panel;
    public GameObject Panel2;
    public GameObject Scroll_View;
    public TextMeshProUGUI announce_text;

    private Dictionary<int, (Employee employee, ProgressCircle progressCircle)> Employees;
    private int CurrentID;

    public ItemScript currentBlanket;
    public ItemScript currentYarn;
    public CottonPanel cottonPanel;
    public FabricDetailPanelController detailPanelController;

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

        bool can_make = false;

        // 딕셔너리에서 현재 직원을 가져옵니다.
        Employee current_employee = Employees[CurrentID].employee;
        ProgressCircle progress_circle = Employees[CurrentID].progressCircle;

        // 람다식에 사용할 로컬 변수 생성
        Employee employeeForLambda = current_employee;

        if (current_employee.lackStamina())
        {
            Debug.Log("스태미너가 부족합니다!");
            ShowAnnounceText("스태미너가 부족합니다.", 2f);
            return;
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

            // 안전하게 로컬 변수에 저장
            Employee employeeCopy = employeeForLambda;
            ItemScript blanketCopy = currentBlanket;

            progress_circle.OnComplete = () =>
            {
                gameManager.Set_Worker_WorkingPercent(employeeCopy.EmployeeID, 0f);

                if (blanketCopy != null)
                {
                    Debug.Log(blanketCopy.yarnName + "만듦");
                }
                else
                {
                    Debug.LogWarning("blanketCopy가 null입니다!");
                }

                showfabric(employeeCopy);
            };

            progress_circle.CompleteCircle(current_employee.EmployeeID);
        }
        else
        {
            ShowAnnounceText("이미 작업 중입니다.", 2f);
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

                if (employee.workingPercent > 0)
                {
                    gameManager.Set_Worker_WorkingPercent(employee.EmployeeID, 0);
                    employee.workingPercent = 0;
                }

                gameManager.Set_Worker_workingItem(employee.EmployeeID, null);
                gameManager.Add_InventoryItem(employee.workItem.itemName, 1);

                employee.workItem = null;
                progress_circle.elapsed = 0f;
                employee.isWorking = false;

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
