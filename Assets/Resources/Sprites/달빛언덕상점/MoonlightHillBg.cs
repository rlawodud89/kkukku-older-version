using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoonlightHillBg : MonoBehaviour
{
    public Image BgImage;
    public Sprite daySprite;
    public Sprite eveningSprite;
    public Sprite nightSprite;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
        ChangeBg(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += ChangeBg;
    }

    private void OnDisable()
    {
        gameManager.OnBgTimeChanged -= ChangeBg;
    }

    private void ChangeBg(BgType bgType)
    {
        if (bgType == BgType.DAY)
        {
            BgImage.sprite = daySprite;
        }
        else if (bgType == BgType.EVENING)
        {
            BgImage.sprite = eveningSprite;
        }
        else
        {
            BgImage.sprite = nightSprite;
        }
    }


}
