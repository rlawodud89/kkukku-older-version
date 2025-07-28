using UnityEngine;
using UnityEngine.Tilemaps;

public class DraggableObject : MonoBehaviour
{
    public Tilemap tilemap;          // 타일맵 기준
    private Vector3 offset;
    private bool dragging = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        dragging = true;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        offset = transform.position - mouseWorld;
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void Update()
    {
        if (dragging)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            Vector3 dragPos = mouseWorld + offset;

            // 드래그된 위치를 타일맵의 셀로 변환
            Vector3Int cellPos = tilemap.WorldToCell(dragPos);
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);

            transform.position = cellCenter;
        }
    }
}
