using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    public GameObject popupPrefab;
    private GameObject currentPopup;
    public int tableID;

    private GameManager gameManager;
    private InteriorScript tableScript;
    private SpriteRenderer spriteRenderer;
    bool hasSelfCollider;


    void Start()
    {
        gameManager = GameManager.getInstance();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasSelfCollider = (GetComponent<Collider2D>() != null) || (GetComponent<Collider>() != null);

        (InteriorScript table, bool isFull) current_table_data = gameManager.Get_Current_Table(tableID);
        tableScript = current_table_data.table;

        if (spriteRenderer && tableScript != null) // 렌더러 있을때만 변경
            Change_Table_image(current_table_data.isFull);
    }

    void OnMouseDown()
    {
        if (gameManager.isDayEndPanel) return;

        // 마우스가 UI 위에 있을 경우 → 클릭 무시
        if (IsPointerOverUI())
        {
            Debug.Log("UI 위 클릭 → Table 클릭 무시");
            return;
        }

        if (popupPrefab == null) return;

        if (currentPopup != null)
        {
            Destroy(currentPopup);
        }

        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogWarning("Canvas is not in Screen Space - Overlay mode.");
            return;
        }

        // 1. 오브젝트 월드 위치 → 화면 픽셀 좌표
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // 2. 팝업 생성 및 위치 설정
        currentPopup = Instantiate(popupPrefab, canvas.transform);
        TablePanel popupPanel = currentPopup.GetComponent<TablePanel>();
        popupPanel.tableID = tableID;
        popupPanel.OnFullChanged += Change_Table_image;


        RectTransform rect = currentPopup.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0f); // 중심 아래
        rect.position = screenPos; // 바로 픽셀 좌표로 설정 (WorldToScreenPoint 결과 사용)
    }

    // 실제 UI 위에 있는지 확인하는 정밀 메서드
    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster gr = FindObjectOfType<GraphicRaycaster>();

        if (gr != null)
        {
            gr.Raycast(eventData, results);
            return results.Count > 0;
        }

        return false;
    }

    private void Change_Table_image(bool isFull)
    {
        if (isFull) spriteRenderer.sprite = tableScript.fullImage;
        else spriteRenderer.sprite = tableScript.image;
    }
}