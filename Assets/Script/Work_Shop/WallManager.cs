using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallManager : MonoBehaviour
{
    public Tilemap wallTilemap;
    public TileBase selectedWallTile;

    // Start is called before the first frame update
    void Start()
    {
        ReplaceAllWallTiles(); //테스트용
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReplaceAllWallTiles()
    {
        foreach (var pos in wallTilemap.cellBounds.allPositionsWithin) //타일맵에 존재하는 모든 좌표 검색
        {
            if (wallTilemap.HasTile(pos)) //벽 타일 있는 곳에 전부 변경
            {
                wallTilemap.SetTile(pos, selectedWallTile);
            }
        }
    }
}
