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

    private GameManager gameManager;
    private bool can_make = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickMakebtn()
    {
        if (can_make)
        {

       
            Panel.SetActive(false);
            Panel2.SetActive(false);
            Scroll_View.SetActive(false);


            Employee1.Working();

            progresscircle.OnComplete = () =>
            {
                Debug.Log("complete");
                showfabric(); 
            };

            progresscircle.CompleteCircle();
        }

    }

    public Sprite GetMaterialImage(RecipeEntry entry)
    {
        try
        {
            ItemScript material = gameManager.Get_Material(entry.itemName);
            return material.image;
        }
        catch (KeyNotFoundException)
        {
            Debug.LogWarning($"재료 '{entry.itemName}'이 Materials 딕셔너리에 없습니다.");
            return null;
        }
    }

    void showfabric()
    {
        if (currentBlanket != null)
        {
            Debug.Log(currentBlanket.itemName + "의 원단");

            BallonPanel.SetActive(true);
            FabricButton.gameObject.SetActive(true);
            //FabricButton.image.sprite =;

            FabricButton.onClick.RemoveAllListeners();
            FabricButton.onClick.AddListener(() =>
            {
                //currentBlanket.FabricCount += 1;
                //Debug.Log($"{currentBlanket.BlanketName} 원단 수량: {currentBlanket.FabricCount}");

          
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
