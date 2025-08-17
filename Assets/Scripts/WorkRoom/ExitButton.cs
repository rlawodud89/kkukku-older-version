using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject Panel;
    public Make_Fabric makeFabric;

    private FabricDetailPanelController detailPanelController;
    public void CloseScrollView()
    {
        if (makeFabric==null)
        {
            makeFabric = FindObjectOfType<Make_Fabric>();
        }

        if (makeFabric.currentBlanket!=null)
        {
            makeFabric.currentBlanket = null;

            if (detailPanelController == null)
            {
                detailPanelController = FindObjectOfType<FabricDetailPanelController>();
            }
            detailPanelController.ResetAllSlots();
        }

        scrollView.SetActive(false);
        Panel.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
