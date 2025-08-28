using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CloundPondBg : MonoBehaviour
{
    public Image BgImage;
    public Sprite daySprite;
    public Sprite eveningSprite;
    public Sprite nightSprite;
    public List<Image> Objects;

    private GameManager gameManager;
    private Color dayColor = new Color(0.74f, 1.00f, 0.98f);
    private Color eveningColor = new Color(1.8f, 0.8f, 0.8f);
    private Color nightColor = new Color(0.40f, 0.61f, 0.82f);

    void Start()
    {
        gameManager = GameManager.getInstance();
        ChangeBg(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += ChangeBg;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameManager = GameManager.getInstance();
        ChangeBg(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += ChangeBg;
    }

    private void ChangeBg(BgType bgType)
    {
        if (BgImage == null) return;

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
