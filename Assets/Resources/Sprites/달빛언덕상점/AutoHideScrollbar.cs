using UnityEngine;
using UnityEngine.UI;

public class AutoScrollbarHider : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Scrollbar scrollbar;

    void LateUpdate()
    {
        if (scrollRect.content == null) return;

        float contentHeight = scrollRect.content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        scrollbar.gameObject.SetActive(contentHeight > viewportHeight);
    }
}
