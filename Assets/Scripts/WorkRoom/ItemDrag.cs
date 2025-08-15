using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemScript itemData; // 연결된 간식 데이터
    public Canvas dragCanvas;   // 드래그 표시용 Canvas (UI 최상위)

    private GameObject dragClone;         // 드래그 중인 복제 아이콘
    private RectTransform dragCloneRect;  // 복제 아이콘 RectTransform
    private CanvasGroup dragCloneGroup;   // 복제 아이콘 CanvasGroup

    private bool droppedOnEmployee = false;

    void Awake()
    {
        if (dragCanvas == null)
        {
            dragCanvas = GetComponentInParent<Canvas>();
            if (dragCanvas == null)
                dragCanvas = FindObjectOfType<Canvas>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Image img = GetComponent<Image>();
        if (img == null || img.sprite == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        // 슬롯 원본 대신 복제본 생성
        dragClone = new GameObject("DragIcon");
        dragClone.transform.SetParent(dragCanvas.transform, false);
        dragClone.transform.SetAsLastSibling();

        Image cloneImg = dragClone.AddComponent<Image>();
        cloneImg.sprite = img.sprite;
        cloneImg.raycastTarget = false; // 드래그 중 클릭 방지

        dragCloneRect = dragClone.GetComponent<RectTransform>();
        dragCloneRect.sizeDelta = img.rectTransform.sizeDelta;

        dragCloneGroup = dragClone.AddComponent<CanvasGroup>();
        dragCloneGroup.blocksRaycasts = false;

        droppedOnEmployee = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCloneRect == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragCanvas.transform as RectTransform,
            eventData.position,
            dragCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : dragCanvas.worldCamera,
            out localPoint);

        dragCloneRect.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragClone != null)
            Destroy(dragClone);
    }

    public void MarkAsDropped()
    {
        droppedOnEmployee = true;
    }
}
