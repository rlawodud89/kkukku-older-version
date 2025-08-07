using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorManager : MonoBehaviour
{
    public GameObject interiorPanel;
    public GameObject ItemButtonPrefab;
    public GameObject scrollContent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CategorySelect(string category)
    {   
        switch (category)
        {
            case "가구":
                //Debug.Log("가구 category selected.");
                SetFurnitureItem();
                break;
            case "타일":
                //Debug.Log("타일 category selected.");
                SetTileItem();
                break;
            case "직원":
                //Debug.Log("직원 category selected.");
                SetEmployeeItem();
                break;
        }
    }

    public void SetFurnitureItem()
    {
        Clear(); 
        GameObject ItemButton = Instantiate(ItemButtonPrefab, scrollContent.transform);
        GameObject ItemButton2 = Instantiate(ItemButtonPrefab, scrollContent.transform);
        GameObject ItemButton3 = Instantiate(ItemButtonPrefab, scrollContent.transform);
        GameObject ItemButton4 = Instantiate(ItemButtonPrefab, scrollContent.transform);

    }

    public void SetTileItem()
    {
        Clear(); 
        GameObject ItemButton = Instantiate(ItemButtonPrefab, scrollContent.transform);
        GameObject ItemButton2 = Instantiate(ItemButtonPrefab, scrollContent.transform);
        GameObject ItemButton3 = Instantiate(ItemButtonPrefab, scrollContent.transform);
    }

    public void SetEmployeeItem()
    {
        Clear(); 
    }

    public void Clear(){
        foreach (Transform child in scrollContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void PanelOpen()
    {
        if (interiorPanel != null)
        {
            interiorPanel.SetActive(true);
            SetFurnitureItem();
        }
    }

    public void PanelClose()
    {
        if (interiorPanel != null)
        {
            interiorPanel.SetActive(false);
        }
    }
}
