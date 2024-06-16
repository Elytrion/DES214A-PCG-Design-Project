using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TileMapVisualizer : MonoBehaviour
{
    public TileBaseSet FloorTileBaseSet;
    public TileBaseSet WallTileBaseSet;

    public Tilemap FloorTileMap;
    public Tilemap WallTileMap;

    public void PaintFloorTiles(IEnumerable<Vector2Int> inPositions)
    {
        PaintTiles(inPositions, FloorTileMap, FloorTileBaseSet.TilesToUse[0].TileToUse);
    }
    
    public void PaintWallTiles(Dictionary<Vector2Int, int> inPositionsWithData)
    {
        foreach (var pos in inPositionsWithData)
        {
            TileBase tileToUse = WallTileBaseSet.GetTileWithLabel(pos.Value);
            PaintSingleTile(pos.Key, WallTileMap, tileToUse);
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> inPositions, Tilemap inTileMap, TileBase inTileToUse)
    {
        foreach (var pos in inPositions)
        {
            PaintSingleTile(pos, inTileMap, inTileToUse);
        }
    }

    private void PaintSingleTile(Vector2Int inPos, Tilemap inTileMap, TileBase inTileToUse)
    {
        var tileWorldPos = inTileMap.WorldToCell((Vector3Int)inPos);
        inTileMap.SetTile(tileWorldPos, inTileToUse);
    }

    public void Clear()
    {
        FloorTileMap.ClearAllTiles();
        WallTileMap.ClearAllTiles();
    }

}
