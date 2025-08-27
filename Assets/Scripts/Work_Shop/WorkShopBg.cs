using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WorkShopBg : MonoBehaviour
{
    public GameObject dayBg;

    private Color dayColor = Color.white;                    // 낮: 원본 그대로
    private Color eveningColor = new Color(0.757f, 0.561f, 0.561f);   // 붉은 노을
    private Color nightColor = new Color(0.357f, 0.361f, 0.416f);    // 거의 검은 밤

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();

        NowtimeChanged(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += NowtimeChanged;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        gameManager.OnBgTimeChanged -= NowtimeChanged;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameManager = GameManager.getInstance();

        NowtimeChanged(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += NowtimeChanged;
    }

    private void NowtimeChanged(BgType nowtime)
    {
        SpriteRenderer sr = dayBg.GetComponent<SpriteRenderer>();

        if (nowtime == BgType.DAY)
        {
            sr.color = dayColor;
        }
        else if (nowtime == BgType.EVENING)
        {
            sr.color = eveningColor;
        }
        else // BgType.NIGHT
        {
            sr.color = nightColor;
        }
    }

}
