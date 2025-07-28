using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSign : MonoBehaviour
{
    public Sprite openSprite;
    public Sprite closeSprite;
    private bool open;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        open = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = closeSprite;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Sign click");

        if(spriteRenderer != null)
        {
            if (open)
            {
                open = false;
                spriteRenderer.sprite = closeSprite;
            }
            else
            {
                open = true;
                spriteRenderer.sprite = openSprite;
            }
        }
    }

}
