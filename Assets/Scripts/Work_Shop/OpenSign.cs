using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        gameManager.OnOpenChanged -= UpdateSign;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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
