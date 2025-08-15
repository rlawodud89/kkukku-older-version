using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class Make_Fabric : MonoBehaviour
{

    public GameObject Panel;
    public GameObject Panel2;
    public GameObject Scroll_View;

    public GameObject BallonPanel;
    public Button FabricButton;
    
    public Employee Employee1;
    public ProgressCircle progresscircle;

    public ItemScript currentBlanket;
    public CottonPanel cottonPanel;
    public FabricDetailPanelController detailPanelController;

    private GameManager gameManager;
    private bool can_make = false;

    // Start is called before the first frame update
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


        //디버깅용
        gameManager.Add_InventoryItem("운무솜", 3);
        gameManager.Add_InventoryItem("꿈실", 3);
        gameManager.Add_InventoryItem("달조각", 3);
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
                gameManager.Add_InventoryItem(currentBlanket.yarnName,1); //원단 추가
                Debug.Log(currentBlanket.yarnName + "만듦");
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
