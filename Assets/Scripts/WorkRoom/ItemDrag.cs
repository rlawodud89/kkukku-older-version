using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image), typeof(CanvasGroup), typeof(RectTransform))]
public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler

{
    public SnacksData itemData; // 연결된 간식 데이터

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;

    public Canvas dragCanvas;  // Inspector에서 할당하거나 Awake에서 자동 찾기

    private bool droppedOnEmployee = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (dragCanvas == null)
        {
            // 최상위 Canvas 찾기 (없으면 씬에서 Canvas 찾아서 할당)
            dragCanvas = GetComponentInParent<Canvas>();
            if (dragCanvas == null)
            {
                dragCanvas = FindObjectOfType<Canvas>();
            }
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
        originalPosition = rectTransform.anchoredPosition; // 부모 기준 좌표 저장
        originalParent = transform.parent;

        // dragCanvas 아래로 이동 (드래그 아이템이 항상 최상단으로 렌더링)
        transform.SetParent(dragCanvas.transform, false);

        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        droppedOnEmployee = false;

        transform.SetAsLastSibling(); // 최상단으로 위치
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragCanvas.transform as RectTransform,
            eventData.position,
            dragCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : dragCanvas.worldCamera,
            out localPoint);

        rectTransform.anchoredPosition = localPoint;
    }


    // 드래그 끝날 때 위치 기준으로 직원 감지
    // 드래그 끝날 때 위치 기준으로 직원 감지
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // 드래그 끝난 지점에서 Ray 쏘기
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.CompareTag("Employee"))
        {
            var dropZone = hit.GetComponent<EmployeeDropZone>();
            if (dropZone != null)
            {
                dropZone.OnDropFromDrag(this);
                return; // 여기서 Destroy는 DropZone 쪽에서만 하도록
            }
        }


        // 실패: 원위치로
        StartCoroutine(SnapBack());
    }



    IEnumerator SnapBack()
    {
        transform.SetParent(originalParent, false);
        yield return null;

        float duration = 0.2f;
        float time = 0f;
        Vector3 start = rectTransform.anchoredPosition;

        // anchoredPosition 기준 복귀
        while (time < duration)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(start, originalPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
    }




    public void MarkAsDropped()
    {
        droppedOnEmployee = true;
    }
}  