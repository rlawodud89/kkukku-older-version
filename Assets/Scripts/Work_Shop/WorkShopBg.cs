using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorkShopBg : MonoBehaviour
{
    public Sprite daySprite;
    public Sprite eveningSprite;
    public Sprite nightSprite;
    
    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
        spriteRenderer = GetComponent<SpriteRenderer>();

        NowtimeChanged(gameManager.Get_BgTime());
        gameManager.OnBgTimeChanged += NowtimeChanged;
    }

    private void NowtimeChanged(BgType nowtime)
    {
        if(nowtime == BgType.DAY)
        {
            spriteRenderer.sprite = daySprite;
        }
        else if(nowtime == BgType.EVENING)
        {
            spriteRenderer.sprite = eveningSprite;
        }
        else
        {
            spriteRenderer.sprite = nightSprite;
        }
    }

}
