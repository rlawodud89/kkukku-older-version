using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap floorTilemap;
    public TileBase selectedFloorTile;
    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) // 왼쪽 클릭
            && !EventSystem.current.IsPointerOverGameObject()) 
        {
            //클릭한 곳 좌표 추출
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // 카메라로부터 일정 거리 (카메라가 z=-10이면 10)
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);
            Vector3Int cellPos = floorTilemap.WorldToCell(worldPos);

            if (floorTilemap.HasTile(cellPos)) //미리 타일 깔아둔 곳에만 타일 변경할 수 있도록
            {
                floorTilemap.SetTile(cellPos, selectedFloorTile);
            }

        }
    }

}
