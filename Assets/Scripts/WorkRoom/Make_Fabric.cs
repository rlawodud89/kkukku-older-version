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


    private ProgressCircle progresscircle;
    private Employee Employee1;
    private GameManager gameManager;
    private bool can_make = false;


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
            }

        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickMakebtn()
    {
        can_make = Check_Recipe(currentBlanket);
        Debug.Log(can_make);

        if (can_make)
        {
            for (int i = 0; i < currentBlanket.recipe.Count; i++)
            {
                gameManager.Use_InventoryItem(currentBlanket.recipe[i].itemName, currentBlanket.recipe[i].count);
                Debug.Log(currentBlanket.recipe[i].itemName+ currentBlanket.recipe[i].count+"만큼 감소");
            }

            if (detailPanelController == null)
            {
                detailPanelController = FindObjectOfType<FabricDetailPanelController>();
            }
            detailPanelController.OpenPanel(currentBlanket);


            Panel.SetActive(false);
            Panel2.SetActive(false);
            Scroll_View.SetActive(false);


            Employee1.Working();

            progresscircle.OnComplete = () =>
            {
                showfabric(); 
            };

            progresscircle.CompleteCircle();
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


    void showfabric()
    {
        if (currentBlanket != null)
        {

            BallonPanel.SetActive(true);
            FabricButton.gameObject.SetActive(true);
            FabricButton.image.sprite = gameManager.Blanket_to_Yarn(currentBlanket.itemName).image;

            FabricButton.onClick.RemoveAllListeners();
            FabricButton.onClick.AddListener(() =>
            {
                Debug.Log("버튼 눌림!");
                gameManager.Add_InventoryItem(currentBlanket.yarnName, 1); //원단 추가
                Debug.Log(currentBlanket.yarnName + "만듦");

                cottonPanel?.SetSelectedBlanket(currentBlanket);

                BallonPanel.SetActive(false);
                FabricButton.gameObject.SetActive(false);
                progresscircle.ProgressInit();

            });

        }
        else
        {
            Debug.Log("null");
        }
    }

}
