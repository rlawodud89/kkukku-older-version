using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Make_Fabric : MonoBehaviour
{

    public GameObject Panel;
    public GameObject Panel2;
    public GameObject Scroll_View;
    public CottonPanel cottonPanel;
    public GameObject BallonPanel;
    public Button FabricButton;

    [Header("꼭 연결 안해도됨")]
    public ItemScript currentBlanket;
    public FabricDetailPanelController detailPanelController;
    public ItemScript makingBlanket; // 실제 제작 중인 블랭킷


    private ProgressCircle progresscircle;
    private Employee Employee1;
    private GameManager gameManager;
    private bool can_make = false;
    public bool isMaking = false;


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


        if (progresscircle == null)
        {
            if (Employee1 == null)
            {
                GameObject empObj = GameObject.Find("Employee1(Clone)");
                if (empObj != null)
                    Employee1 = empObj.GetComponent<Employee>();
            }

            if (Employee1 != null)
            {
                progresscircle = Employee1.GetComponentInChildren<ProgressCircle>();

                if (BallonPanel != null)
                    Employee1.SetBallonPanel(BallonPanel);
            }

        }

    }


    public void ClickMakebtn()
    {
        if (isMaking)
        {
            Debug.Log("이미 제작 중입니다!");
            return; // 중복 클릭 방지
        }

        if (currentBlanket == null) return;

        can_make = Check_Recipe(currentBlanket);
        Debug.Log(can_make);

        if (!can_make)
        {
            Debug.Log("제작할 수 없습니다!");
            return;
        }

        // 제작 시작
        isMaking = true;

        // 제작용 변수 분리
        ItemScript makingBlanket = currentBlanket;

        // 재료 차감
        for (int i = 0; i < makingBlanket.recipe.Count; i++)
        {
            gameManager.Use_InventoryItem(makingBlanket.recipe[i].itemName, makingBlanket.recipe[i].count);
            Debug.Log($"{makingBlanket.recipe[i].itemName} {makingBlanket.recipe[i].count}만큼 감소");
        }

        // 상세 패널 열기
        if (detailPanelController == null)
            detailPanelController = FindObjectOfType<FabricDetailPanelController>();

        detailPanelController.OpenPanel(makingBlanket);

        // UI 숨기기
        Panel.SetActive(false);
        Panel2.SetActive(false);
        Scroll_View.SetActive(false);

        // 직원 제작 시작
        Employee1.Working();

        // 프로그레스 완료 시 동작
        progresscircle.OnComplete = () =>
        {
            showfabric(makingBlanket);
        };

        progresscircle.CompleteCircle();
        can_make = false;
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


    void showfabric(ItemScript makingBlanket)
    {
        if (makingBlanket == null)
        {
            Debug.Log("makingBlanket is null");
            return;
        }

        BallonPanel.SetActive(true);
        FabricButton.gameObject.SetActive(true);
        FabricButton.image.sprite = gameManager.Blanket_to_Yarn(makingBlanket.itemName).image;

        FabricButton.onClick.RemoveAllListeners();
        FabricButton.onClick.AddListener(() =>
        {
            Debug.Log("버튼 눌림!");
            gameManager.Add_InventoryItem(makingBlanket.yarnName, 1); // 원단 추가
            cottonPanel?.SetSelectedBlanket(makingBlanket);


            isMaking = false;
            BallonPanel.SetActive(false);
            FabricButton.gameObject.SetActive(false);
            progresscircle.ProgressInit();
        });
    }



}
