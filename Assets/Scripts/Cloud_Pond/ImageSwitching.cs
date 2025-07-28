using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSwitching : MonoBehaviour
{
    public Image targetImage;        // 보여줄 UI Image
    public Sprite[] sprites;         // 0: 이미지1, 1: 이미지2, 2: 이미지3
    public float interval = 0.5f;    // 이미지 전환 간격

    public Sprite begin_Sprite;
    private int index = 0;
    private int direction = 1;

    private bool fishing = false;
    private Coroutine imageLoopCoroutine = null;

    public void Clickfishing()
    {
        if (!fishing)
        {
            fishing = true;
            imageLoopCoroutine = StartCoroutine(ImageLoop());
        }
    }

    public void Clickfishingstop()
    {
        if (fishing)
        {
            fishing = false;

            if (imageLoopCoroutine != null)
            {
                StopCoroutine(imageLoopCoroutine);
                imageLoopCoroutine = null;
            }

            targetImage.sprite = begin_Sprite;
            index = 0;
            direction = 1;
        }
    }

    IEnumerator ImageLoop()
    {
        while (true)
        {
            targetImage.sprite = sprites[index];

            yield return new WaitForSeconds(interval);

            index += direction;

            // 순서: 0 → 1 → 2 → 1 → 0 → 1 → 2 → ...
            if (index == sprites.Length - 1 || index == 0)
            {
                direction *= -1; // 방향 반전
            }
        }
    }
}
