using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepingGardenBg : MonoBehaviour
{
    public Image BgImage;
    public Sprite daySprite;
    public Sprite eveningSprite;
    public Sprite nightSprite;
    public List<Image> Objects;

    private GameManager gameManager;
    Color dayColor = Color.white;
    Color eveningColor = new Color(1.1f, 0.6f, 0.55f);
    Color nightColor = new Color(0.3f, 0.3f, 1.1f);

    void Start()
    {
        gameManager = GameManager.getInstance();
        ChangeBg(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += ChangeBg;
    }

    private void ChangeBg(BgType bgType)
    {
        if (bgType == BgType.DAY)
        {
            BgImage.sprite = daySprite;
            foreach (Image t in Objects)
            {
                t.color = dayColor;
            }
        }
        else if (bgType == BgType.EVENING)
        {
            BgImage.sprite = eveningSprite;
            foreach (Image t in Objects)
            {
                t.color = eveningColor;
            }
        }
        else
        {
            BgImage.sprite = nightSprite;
            foreach (Image t in Objects)
            {
                t.color = nightColor;
            }
        }
    }
}
