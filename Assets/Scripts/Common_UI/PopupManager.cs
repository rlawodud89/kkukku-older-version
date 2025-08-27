using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private List<GameObject> allPopups;

    private AudioManager audioManager;

    void Start(){
        audioManager = AudioManager.Instance;
    }


    public void ShowOnly(GameObject targetPopup)
    {
        audioManager.PlaySFX("pop");

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
