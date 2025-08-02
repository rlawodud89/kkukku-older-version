using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject Panel;
    public void CloseScrollView()
    {
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
