using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Employee : MonoBehaviour
{
    public GameObject ballonPanel;
    public TextMeshProUGUI floatingText;
    public Button ItemButton;

    public System.Action OnWorkComplete;
    public bool isWorking { get; set; } = false;

    public Staminar staminar;
    public SnacksInventory snacksInventory;
    public ProgressCircle progressCircle;

    public string EmployeeName;
    public int EmployeeID;
    public ItemScript workItem;
    public float workingPercent;

    private GameManager gameManager;


    void Start()
    {
        gameManager = GameManager.getInstance();

        if (progressCircle != null)
        {
            progressCircle.OnTaskComplete = () => {
                if (OnWorkComplete != null)
                {
                    // ProgressCircle의 작업이 완료되면, OnWorkComplete 이벤트를 호출합니다.
                    OnWorkComplete.Invoke();
                }
            };
        }
    }

    void Update()
    {

        if (progressCircle.elapsed > 0 && progressCircle.elapsed < progressCircle.maxProgress)
            isWorking = true;
        else
            isWorking = false;
    }


    public void InitializeWorker()
    {
        if (gameManager == null)
        {
            gameManager=GameManager.getInstance();
        }
        // Worker의 위치를 기반으로 GameManager에서 저장된 정보를 불러옵니다.
        // 여기서 x, y는 Worker 오브젝트의 위치를 사용한다고 가정합니다.
        float x = this.transform.position.x;
        float y = this.transform.position.y;

        (int workerID, int stamina, DateTime startTime, ItemScript workItem, float workingPercent) = gameManager.Get_Worker_Info(x, y);
        this.workItem = workItem;
        this.workingPercent= workingPercent;

        Debug.Log(workerID +" "+ workingPercent);

        progressCircle.RefreshMaxProgress();

        // [핵심 수정] 작업 시작 시간이 있다면 진행도를 다시 계산합니다.
        if (startTime != DateTime.MinValue && workingPercent < progressCircle.maxProgress)
        {
            // 씬을 나간 시간과 작업 시작 시간의 차이를 계산합니다.
            double timePassed = DateTime.Now.Subtract(startTime).TotalSeconds;

            // 지난 시간을 기존 진행도에 더합니다.
            workingPercent += (float)timePassed;

            // 진행도가 최대치를 넘기지 않도록 합니다.
            workingPercent = Mathf.Min(workingPercent, progressCircle.maxProgress);
            gameManager.Set_Worker_WorkingPercent(EmployeeID, workingPercent);

        }


        if (workItem == null || workingPercent <= 0)
        {
            progressCircle.elapsed = 0f;
            isWorking = false;
        }

        // 1. 작업이 '진행 중'일 때
        // workingPercent가 0보다 크고, 아직 완료되지 않았을 경우
        if (workItem != null && workingPercent > 0 && workingPercent < progressCircle.maxProgress)
        {
            progressCircle.ProgressInit();
            progressCircle.CompleteCircle(workerID, workingPercent);
        }


        // 2. 작업이 '완료'되었을 때
        // workingPercent가 maxProgress와 같거나 클 경우
        else if (workItem != null && workingPercent >= progressCircle.maxProgress)
        {
            if (OnWorkComplete != null)
            {
                OnWorkComplete.Invoke();
            }

            progressCircle.ProgressInit();
            progressCircle.completeImage.gameObject.SetActive(true);
            progressCircle.fillImage.gameObject.SetActive(false);
            progressCircle.Image.gameObject.SetActive(false);
        }
        // 3. 작업이 '없는' 상태이거나, 진행도가 0일 때
        else
        {
            progressCircle.ProgressInit();

        }
    }



    public void GiveItem(ItemScript item)
    {
        snacksInventory.GiveSnackToEmployee(item);
        Debug.Log("GiveSnackToEmployee 호출됨"+item.value);

        if (staminar.currentStamina+item.value > staminar.maxStamina)
        {
            Debug.Log("스태미너가 충분합니다!");
        }
        staminar.Addstamina(item.value);
        ShowFloatingText("+" + item.value);

        gameManager.Change_Worker_Stamina(EmployeeID, item.value);
    }


    public bool lackStamina()
    {
        if (staminar.currentStamina < 5)
        {
            return true;
        }
        return false;

    }

    public void Working() { 
        staminar.Addstamina(-5);
        gameManager.Change_Worker_Stamina(EmployeeID, -5);
    }



    public void ShowFloatingText(string text)
    {
        floatingText.text = text;
        floatingText.gameObject.SetActive(true);
        // 간단한 fade out 애니메이션 추가 가능
        ballonPanel.SetActive(true);
        Invoke(nameof(HideFloatingText), 1.5f);
    }

    public void HideFloatingText()
    {
        floatingText.gameObject.SetActive(false);
        ballonPanel.SetActive(false);
    }

}
