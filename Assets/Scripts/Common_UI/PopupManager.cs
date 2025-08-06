using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private List<GameObject> allPopups;


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
}
