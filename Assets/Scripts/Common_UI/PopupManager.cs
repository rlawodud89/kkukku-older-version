using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private List<GameObject> allPopups;

    public Transform popupContainer; // 빈 부모 오브젝트
    
    private List<GameObject> popupList = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowOnly(GameObject targetPopup)
    {
        foreach (var popup in allPopups)
        {
            popup.SetActive(false);
        }

        if (targetPopup != null)
            targetPopup.SetActive(true);   
        
    }

    public void HideAll()
    {
        foreach (var popup in allPopups)
        {
            popup.SetActive(false);
        }
    } 

    
    public void ShowPopup(GameObject popupPrefab)
    {
        HideAllPopups(); // 기존 거 다 끔

        GameObject popup = Instantiate(popupPrefab, popupContainer);
        popup.SetActive(true);
        popupList.Add(popup);
    }

    
    public void HideAllPopups()
    {
        foreach (Transform child in popupContainer)
        {
            child.gameObject.SetActive(false);
        }
    }
}
