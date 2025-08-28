using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Make_Sewing : MonoBehaviour
{
    public static Make_Sewing Instance { get; private set; }

    public GameObject sewingPanel;
    public SewingPanel sewing_panel;

    private Dictionary<int, (Employee employee, ProgressCircle progressCircle)> Employees;
    private int CurrentID;

    public GameObject BallonPanel;
    public GameObject CompletePanel;

    public TextMeshProUGUI announce_text;
    public Button SewingButton;
    public Image CompleteImage;
    public TextMeshProUGUI CompleteText;

    private GameManager gameManager;
    private ItemScript currentBlanket;


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


        gameManager.Use_InventoryItem(currentSewing.itemName, 1);

        sewing_panel.RefreshInventoryUI();

        currentBlanket = gameManager.Cotton_to_Blanket(currentSewing.itemName);
        current_employee.workItem = currentBlanket;
        gameManager.Set_Worker_workingItem(current_employee.EmployeeID, currentBlanket.itemName);

        sewingPanel.SetActive(false);

        current_employee.Working();

        progress_circle.CompleteCircle(current_employee.EmployeeID);
    }

    // showsewing 함수가 Employee 객체를 인수로 받도록 수정
    public void showsewing(Employee employee)
    {
        ProgressCircle progress_circle = Employees[employee.EmployeeID].progressCircle;
        GameObject ballon_Panel = employee.ballonPanel;
        Button sewing_button = employee.ItemButton;

        if (employee.workItem != null)
        {
            ballon_Panel.SetActive(true);
            sewing_button.gameObject.SetActive(true);
            sewing_button.image.sprite = employee.workItem.image;

            ItemScript finishedItem = employee.workItem;

            sewing_button.onClick.RemoveAllListeners();
            sewing_button.onClick.AddListener(() =>
            {
                ballon_Panel.SetActive(false);
                sewing_button.gameObject.SetActive(false);
                progress_circle.ProgressInit();

                CompletePanel.SetActive(true);
                CompleteImage.sprite = finishedItem.image;
                CompleteText.text = finishedItem.itemName + "이 완성되었습니다!";

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

    public void ClickCompleteBtn()
    {
        CompletePanel.SetActive(false);
        
        // 퀘스트
        AddQuestProcess.Instance.AddProcessToQuest("이불 제작");


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