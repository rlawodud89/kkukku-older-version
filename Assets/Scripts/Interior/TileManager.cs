using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        gameManager.OnTileChanged += TileChanged;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameManager = GameManager.getInstance();
        Sprite current_tile = gameManager.Get_Current_Tile(posType);
        ReplaceAllWallTiles(SpriteToTile(current_tile));

        gameManager.OnTileChanged += TileChanged;
    }

    private void ReplaceAllWallTiles(TileBase selectedWallTile)
    {
        // 실제 배치된 모든 벽 타일 좌표 리스트
        List<Vector3Int> wallPositions = new List<Vector3Int>();

        foreach (var pos in wallTilemap.cellBounds.allPositionsWithin)
        {
            if (wallTilemap.HasTile(pos))
                wallPositions.Add(pos);
        }

        // 필요할 때만 변경
        foreach (var pos in wallPositions)
        {
            TileBase currentTile = wallTilemap.GetTile(pos);
            if (currentTile != selectedWallTile) // 이미 같은 타일이면 교체 안 함
                wallTilemap.SetTile(pos, selectedWallTile);
        }
    }

    private TileBase SpriteToTile(Sprite sprite)
    {
        UnityEngine.Tilemaps.Tile tile = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
        tile.sprite = sprite;
        return tile;
    }

    private void TileChanged(TilePosType posType, InteriorScript tile)
    {
        if (posType != this.posType) return;

        ReplaceAllWallTiles(SpriteToTile(tile.image));
    }

}
