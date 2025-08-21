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
        Debug.Log($"[ItemDrag] 드래그 시작: {gameObject.name}");

        Image img = GetComponent<Image>();
        if (img == null || img.sprite == null)
        {
            Debug.LogWarning("[ItemDrag] 드래그 시작 실패 → Image 또는 Sprite 없음");
            eventData.pointerDrag = null;
            return;
        }

        // 복제 아이콘 생성
        dragClone = new GameObject("DragIcon");
        dragClone.transform.SetParent(dragCanvas.transform, false);
        dragClone.transform.SetAsLastSibling();

        Image cloneImg = dragClone.AddComponent<Image>();
        cloneImg.sprite = img.sprite;
        cloneImg.raycastTarget = false;

        dragCloneRect = dragClone.GetComponent<RectTransform>();
        dragCloneRect.sizeDelta = img.rectTransform.sizeDelta;

        dragCloneGroup = dragClone.AddComponent<CanvasGroup>();
        dragCloneGroup.blocksRaycasts = false;

        droppedOnEmployee = false;
        Debug.Log("[ItemDrag] 드래그 아이콘 생성 완료");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCloneRect == null)
        {
            Debug.LogWarning("[ItemDrag] dragCloneRect 가 NULL → 드래그 아이콘 없음");
            return;
        }

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
        Debug.Log($"[ItemDrag] 드래그 종료. droppedOnEmployee={droppedOnEmployee}");

        if (dragClone != null)
        {
            Destroy(dragClone);
            Debug.Log("[ItemDrag] 드래그 아이콘 삭제");
        }
    }

    public void MarkAsDropped()
    {
        droppedOnEmployee = true;
        Debug.Log("[ItemDrag] 직원에게 정상 드롭됨!");

        if (dragClone != null)
        {
            Destroy(dragClone);
            dragClone = null;
            Debug.Log("[ItemDrag] 드롭 성공 → 드래그 아이콘 삭제");
        }
    }


}
