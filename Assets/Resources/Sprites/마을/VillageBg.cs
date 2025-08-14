using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageBg : MonoBehaviour
{
    public Image BgImage;
    public Sprite daySprite;
    public Sprite eveningSprite;
    public Sprite nightSprite;
    public List<Image> Objects;

    private GameManager gameManager;
    private Color dayColor = new Color(2.0f, 2.0f, 2.0f);
    private Color eveningColor = new Color(1.5f, 0.6f, 0.5f);
    private Color nightColor = Color.white;

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
            foreach(Image t in Objects)
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
