using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomModule : MonoBehaviour
{
    public string ID = null;
    public Tilemap RoomTiles = null;
    private BoundsInt _bounds;
    private bool _boundsSet = false;
    
    public void SetGridBounds(Vector2Int inSpawnPoint)
    {
        Vector2Int gridSize = GetGridSize();
        _bounds = new BoundsInt(new Vector3Int(inSpawnPoint.x, inSpawnPoint.y, 0), new Vector3Int(gridSize.x, gridSize.y, 0));
        _boundsSet = true;
    }

    public BoundsInt GetGridBounds()
    {
        if (_boundsSet)
            return _bounds;

        Debug.LogError("Bounds not set!");
        return new BoundsInt();
    }

    public Vector2Int GetGridSize()
    {
        if (RoomTiles == null)
        {
            Debug.LogError(gameObject.name + "RoomTiles is null!");
            return Vector2Int.zero;
        }

        RoomTiles.CompressBounds();
        return new Vector2Int((int)RoomTiles.cellBounds.size.x, (int)RoomTiles.cellBounds.size.y);
    }
    
    public Vector2Int GetGridCenter()
    {
        BoundsInt selfBounds = GetGridBounds();
        return new Vector2Int((int)selfBounds.center.x, (int)selfBounds.center.y);
    }

    public HashSet<Vector2Int> GetAllTilePositions(Vector2Int inSpawnPos)
    {
        Vector2Int size = GetGridSize();
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
        for (int x = -size.x; x < size.x; x++)
        {
            for (int y = -size.y; y < size.y; y++)
            {
                Vector3Int grid_pos = new Vector3Int(x, y, 0);
                var tile = RoomTiles.GetTile(grid_pos);
                if (tile != null)
                {
                    positions.Add(inSpawnPos + new Vector2Int(x, y));
                }
            }
        }
        return positions;
    }
}
