using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorkShopBg : MonoBehaviour
{
    public GameObject dayBg;
    public GameObject eveningBg;
    public GameObject nightBg;
    
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();

        NowtimeChanged(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += NowtimeChanged;
    }

    private void NowtimeChanged(BgType nowtime)
    {
        if(nowtime == BgType.DAY)
        {
            dayBg.SetActive(true);
            eveningBg.SetActive(false);
            nightBg.SetActive(false);
        }
        else if(nowtime == BgType.EVENING)
        {
            dayBg.SetActive(false);
            eveningBg.SetActive(true);
            nightBg.SetActive(false);
        }
        else // BgType.NIGHT
        {
            dayBg.SetActive(false);
            eveningBg.SetActive(false);
            nightBg.SetActive(true);
        }
    }

}
