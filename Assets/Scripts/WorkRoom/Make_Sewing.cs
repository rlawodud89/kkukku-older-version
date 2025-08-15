using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Make_Sewing : MonoBehaviour
{

    public static Make_Sewing Instance { get; private set; }

    public GameObject sewingPanel;
    public Employee Employee3;
    public ProgressCircle progresscircle;

    public GameObject BallonPanel;
    public GameObject CompletePanel;

    public Button SewingButton;
    public Image CompleteImage;
    public TextMeshProUGUI CompleteText;
    
    private GameManager gameManager;
    private ItemScript currentBlanket;

    private void Start()
    {
        gameManager = GameManager.getInstance(); 
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

    public void HandleMakeClicked(ItemScript currentSewing)
    {


        currentBlanket = gameManager.Cotton_to_Blanket(currentSewing.itemName);
        Debug.Log("Make_Sewing에서 Make 버튼 클릭됨 감지!");
        gameManager.Add_InventoryItem(currentBlanket.cottonName, -1);

        gameManager = GameManager.getInstance();
        sewingPanel.SetActive(false);
        Employee3.Working();


        progresscircle.OnComplete = () =>
        {
            gameManager.Add_InventoryItem(currentBlanket.itemName, 1);
            Debug.Log("완성");
            showsewing(currentBlanket);
        };

        progresscircle.CompleteCircle();


    }

    void showsewing(ItemScript currentBlanket)
    {
        if (currentBlanket  != null)
        {

            BallonPanel.SetActive(true);
            SewingButton.gameObject.SetActive(true);
            SewingButton.image.sprite = currentBlanket.image;

            SewingButton.onClick.RemoveAllListeners();
            SewingButton.onClick.AddListener(() =>
            {

                BallonPanel.SetActive(false);
                SewingButton.gameObject.SetActive(false);
                progresscircle.ProgressInit();

                CompletePanel.SetActive(true);
                CompleteImage.sprite = currentBlanket.image;
                CompleteText.text = currentBlanket.itemName +"이 완성되었습니다!";

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

}
