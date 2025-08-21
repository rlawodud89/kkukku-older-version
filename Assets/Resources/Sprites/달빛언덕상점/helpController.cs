using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helpController : MonoBehaviour
{
    public GameObject helpPanel;
    public void clickhelpbtn()
    {
        helpPanel.SetActive(true);
    }

    public void clickexitbtn()
    {
        helpPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
