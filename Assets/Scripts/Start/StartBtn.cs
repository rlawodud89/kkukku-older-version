using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtn : MonoBehaviour
{
    DBManager dbManager;

    void Start()
    {
        dbManager = DBManager.getInstance();
    }

    public void ClickStartBtn()
    {
        User user = dbManager.Get_User();
        SceneManager.LoadScene(user.endScene);
    }
}
