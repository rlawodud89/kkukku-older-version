using System.Collections.Generic;
using UnityEngine;

public class MaterialStorage : MonoBehaviour
{
    public StoragePanel storagePanel;
    private GameManager gameManager; 

    void Start()
    {
        gameManager = GameManager.getInstance();
        //FillMaterialsToSlots();
    }

}
