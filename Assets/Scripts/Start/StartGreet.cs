using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartGreet : MonoBehaviour
{

    public GameObject greetImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ClickButton(){

        if(greetImage.activeSelf)
            greetImage.SetActive(false);
        else
            greetImage.SetActive(true);
    }
}
