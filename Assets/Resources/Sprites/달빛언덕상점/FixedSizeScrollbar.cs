using UnityEngine;
using UnityEngine.UI;

public class FixedSizeScrollbar : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Scrollbar scrollbar;
    [Range(0f, 1f)] public float fixedSize = 0.2f;

    void Start()
    {
        scrollbar.size = fixedSize;

        scrollRect.onValueChanged.AddListener(OnScrollChanged);
        scrollbar.onValueChanged.AddListener(OnScrollbarDragged);
    }

    void OnScrollChanged(Vector2 normalizedPos)
    {
        scrollbar.value = normalizedPos.y;
    }

    void OnScrollbarDragged(float value)
    {
        // ScrollRect 스크롤 위치 직접 갱신
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, value);
    }
}

