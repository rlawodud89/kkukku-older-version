using System.Collections;
using System.Collections.Generic;
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

    public BlanketData currentBlanket;
    public CottonPanel cottonPanel;

    private bool can_make = true;

    // Start is called before the first frame update
    void Start()
    {
        
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
                showfabric(); 
            };

            progresscircle.CompleteCircle();
        }

    }

    void showfabric()
    {
        if (currentBlanket != null)
        {
            Debug.Log(currentBlanket.BlanketName + "의 원단");

            BallonPanel.SetActive(true);
            FabricButton.gameObject.SetActive(true);
            FabricButton.image.sprite = currentBlanket.Fabric;

            FabricButton.onClick.RemoveAllListeners();
            FabricButton.onClick.AddListener(() =>
            {
                currentBlanket.FabricCount += 1;
                Debug.Log($"{currentBlanket.BlanketName} 원단 수량: {currentBlanket.FabricCount}");

                cottonPanel?.SetSelectedBlanket(currentBlanket);


                BallonPanel.SetActive(false);
                FabricButton.gameObject.SetActive(false);
            });

        }
    }

}
