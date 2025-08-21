using UnityEngine;
using UnityEngine.UI;
public class Make_Cotton : MonoBehaviour
{
    public static Make_Cotton Instance { get; private set; }

    public GameObject cottonPanel;
    public SewingPanel sewingPanel;
    public GameObject BallonPanel;
    public Button CottonButton;

    private ItemScript currentYarn;
    private ItemScript currentCotton;


    private Employee Employee2;
    private ProgressCircle progresscircle;
    private GameManager gameManager;
    public bool isMaking = false;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        if (progresscircle == null)
        {
            if (Employee2 == null)
            {
                GameObject empObj = GameObject.Find("Employee2(Clone)");
                if (empObj != null)
                    Employee2 = empObj.GetComponent<Employee>();
            }

            if (Employee2 != null)
            {   
                progresscircle = Employee2.GetComponentInChildren<ProgressCircle>();


                if (BallonPanel != null)
                    Employee2.SetBallonPanel(BallonPanel);
            }

        }

    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }

        Instance = this;
    }

    public void HandleMakeClicked(ItemScript currentYarn)
    {
        if (isMaking)
        {
            Debug.Log("이미 제작 중입니다!");
            return; // 중복 클릭 방지
        }

        if (gameManager == null)
        {
            gameManager = GameManager.getInstance();
        }

        if (Employee2 == null)
        {
            GameObject empObj = GameObject.Find("Employee2(Clone)");
            if (empObj != null)
                Employee2 = empObj.GetComponent<Employee>();
        }

        Debug.Log("Make_Cotton에서 Make 버튼 클릭됨 감지!");
        gameManager.Add_InventoryItem(currentYarn.itemName, -1);

        currentCotton = gameManager.Yarn_to_Cotton(currentYarn.itemName);

        cottonPanel.SetActive(false);
        // Employee2 작업
        if (Employee2 != null)
        {
            Employee2.Working();
            isMaking = true;
        }
        else
            Debug.LogWarning("Employee2가 null 상태라 Working() 호출 불가");


        progresscircle.OnComplete = () =>
        {
            showcotton();
        };

        progresscircle.CompleteCircle();


    }

    void showcotton()
    {
        if (currentCotton != null)
        {

            BallonPanel.SetActive(true);
            CottonButton.gameObject.SetActive(true);
            CottonButton.image.sprite =currentCotton.image;

            CottonButton.onClick.RemoveAllListeners();
            CottonButton.onClick.AddListener(() =>
            {

                gameManager.Add_InventoryItem(currentCotton.itemName, 1);
                Debug.Log("complete");

                sewingPanel.currentSewing = currentCotton;
                sewingPanel?.SetSelectedBlanket();

                isMaking = false;
                BallonPanel.SetActive(false);
                CottonButton.gameObject.SetActive(false);
                progresscircle.ProgressInit();

            });

        }
        else
        {
            Debug.Log("null");
        }
    }
}
