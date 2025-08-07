using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSign : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;
    private SpriteRenderer spriteRenderer;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.getInstance();
        gameManager.OnOpenChanged += UpdateSign;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (gameManager.Get_IsOpen())
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closeSprite;
        }

    }

    void OnDisable()
    {

        gameManager.OnOpenChanged -= UpdateSign;

    }

    void UpdateSign(bool isOpen)
    {
        if (isOpen)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closeSprite;
        }
    }

}
