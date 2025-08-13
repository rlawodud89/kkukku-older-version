using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileManager : MonoBehaviour
{
    public Tilemap wallTilemap;
    public TilePosType posType;
    private GameManager gameManager;


    void Start()
    {
        gameManager = GameManager.getInstance();
        Sprite current_tile = gameManager.Get_Current_Tile(posType);
        ReplaceAllWallTiles(SpriteToTile(current_tile));
    }


    private void ReplaceAllWallTiles(TileBase selectedWallTile)
    {
        foreach (var pos in wallTilemap.cellBounds.allPositionsWithin) //타일맵에 존재하는 모든 좌표 검색
        {
            if (wallTilemap.HasTile(pos)) //벽 타일 있는 곳에 전부 변경
            {
                wallTilemap.SetTile(pos, selectedWallTile);
            }
        }
    }

    private TileBase SpriteToTile(Sprite sprite)
    {
        UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
        tile.sprite = sprite;
        return tile;
    }


}
