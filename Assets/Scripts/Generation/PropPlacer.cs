using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using PropType = PropPrefabs.PropType;

public class PropPlacer : MonoBehaviour
{
    public enum TileOccupancy { EMPTY, WALL, PROP, NONE};

    [System.Serializable]
    public struct PropPrefab
    {
        public Vector2Int TileSize;
        public GameObject Prop;
        public bool NeedsWall;
        public bool IsASpawnpoint;
        public int RequiredSpacing;
    };

    private Dictionary<RoomSetter, Dictionary<Vector2Int, TileOccupancy>> RoomToTileMapDataDict;
    private HashSet<Vector2Int> AllCorridorPositions;

    public List<RoomSetter> AllRooms;

    public string ValidTileTag = "Floor";

    private void Awake()
    {
        RoomToTileMapDataDict = new Dictionary<RoomSetter, Dictionary<Vector2Int, TileOccupancy>>();
    }

    public bool PlacePropsInAllRooms(List<GameObject> AllInstantiatedRooms, HashSet<Vector2Int> AllCorridorPos)
    {
        AllRooms.Clear();
        AllCorridorPositions = AllCorridorPos;
        foreach (Dictionary<Vector2Int, TileOccupancy> dict in RoomToTileMapDataDict.Values)
        {
            dict.Clear();
        }
        RoomToTileMapDataDict.Clear();  
        foreach (GameObject room in AllInstantiatedRooms)
        {
            RoomSetter roomSetter = room.GetComponent<RoomSetter>();
            if (roomSetter != null)
            {
                AllRooms.Add(roomSetter);
            }
        }

        if (AllRooms.Count <= 0)
            return false;
        bool failure = false;
        foreach (RoomSetter room in AllRooms)
        {
            if (room.CanHaveProps == false)
                continue;

            Dictionary<Vector2Int, TileOccupancy> outAllRoomTiles;
            failure = PlacePropsInRoom(room, out outAllRoomTiles);
            if (!failure)
            {
                RoomToTileMapDataDict.Add(room, outAllRoomTiles);
            }
        }
        return !failure;
    }

    public bool PlacePropsInRoom(RoomSetter inRoom, out Dictionary<Vector2Int, TileOccupancy> outAllRoomTiles)
    {
        outAllRoomTiles = ConvertRoomToTiles(inRoom);
        if (outAllRoomTiles.Count <= 0)
            return false;
        PropPrefabs propPool = inRoom.Props;

        foreach (PropType pt in propPool.AllPropTypes)
        {
            for (int x = 0; x < pt.SpawnCount; x++)
            {
                PlacePropInRoom(inRoom, pt.PropData, outAllRoomTiles);
            }
        }

        return true;
    }

    private Dictionary<Vector2Int, TileOccupancy> ConvertRoomToTiles(RoomSetter inRoom)
    {
        Dictionary<Vector2Int, TileOccupancy> allTiles = new Dictionary<Vector2Int, TileOccupancy>();

        foreach (Tilemap tilemap in inRoom.AllTilemaps)
        {
            var allTilesInMap = GetAllTiles(inRoom, tilemap);
            foreach (var tile in allTilesInMap)
            {
                if (allTiles.ContainsKey(tile.Key))
                {
                    if (tile.Value > allTiles[tile.Key])
                    {
                        allTiles[tile.Key] = tile.Value;
                    }
                }
                else
                {
                    allTiles.Add(tile.Key, tile.Value);
                }
            }
        }

        return allTiles;
    }

    private Dictionary<Vector2Int, TileOccupancy> GetAllTiles(RoomSetter inRoom, Tilemap inTilemap)
    {
        Dictionary<Vector2Int, TileOccupancy> allRoomTiles = new Dictionary<Vector2Int, TileOccupancy>();

        bool validTile = false;
        if (inTilemap.gameObject.tag == ValidTileTag)
        {
            validTile = true;
        }

        inTilemap.CompressBounds();
        var bounds = inTilemap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = inTilemap.GetTile(cellPos);

                if (tile == null)
                    continue;

                Vector2Int tilePos = new Vector2Int((int)inRoom.transform.position.x + x, (int)inRoom.transform.position.y + y);
                TileOccupancy occupancyData = (validTile) ? TileOccupancy.EMPTY : TileOccupancy.WALL;
                allRoomTiles[tilePos] = occupancyData;
            }
        }

        return allRoomTiles;
    }

    public void PlaceSpecificPropInRoom(PropPrefab inSpecificProp, RoomSetter inRoom)
    {
        Dictionary<Vector2Int, TileOccupancy> tileOccupancyData = RoomToTileMapDataDict[inRoom];
        PlacePropInRoom(inRoom, inSpecificProp, tileOccupancyData);
    }
    
    public void MovePropToRoom(PropPrefab inProp, RoomSetter inRoom)
    {
        Dictionary<Vector2Int, TileOccupancy> tileOccupancyData = RoomToTileMapDataDict[inRoom];
        List<Vector2Int> AllValidTiles = (inProp.NeedsWall) ? GetAllTilesNearWalls(tileOccupancyData) : GetAllTilesOpenSpace(tileOccupancyData);
        bool placedProp = false;
        int counter = 0;
        while (!placedProp && counter < 10) // we try at least 10 attempts per prop
        {
            counter++;
            // get a random tile
            if (AllValidTiles.Count <= 0)
                break;
            
            Vector2Int ValidTile = AllValidTiles[Random.Range(0, AllValidTiles.Count - 1)];

            // check if tile is valid
            Vector2Int spawnPos;
            if (CanFitProp(inProp, ValidTile, tileOccupancyData, out spawnPos))
            {
                Vector3 spawnPosFull = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
                inProp.Prop.transform.position = spawnPosFull;
                for (int c = 0; c < inProp.TileSize.x; c++)
                {
                    for (int r = 0; r < inProp.TileSize.y; r++)
                    {
                        Vector2Int tilePos = new Vector2Int(spawnPos.x + c, spawnPos.y + r);
                        if (tileOccupancyData.ContainsKey(tilePos))
                        {
                            tileOccupancyData[tilePos] = TileOccupancy.PROP;
                        }
                    }
                }
                placedProp = true;
                return;
            }
        }
    }

    private void PlacePropInRoom(RoomSetter inRoom, PropPrefab inProp, Dictionary<Vector2Int, TileOccupancy> inTileOccupancyData)
    {
        // get all valid tiles
        List<Vector2Int> AllValidTiles = (inProp.NeedsWall) ? GetAllTilesNearWalls(inTileOccupancyData) : GetAllTilesOpenSpace(inTileOccupancyData);

        bool placedProp = false;
        int counter = 0;
        while (!placedProp && counter < 10) // we try at least 10 attempts per prop
        {
            counter++;
            // get a random tile
            if (AllValidTiles.Count <= 0)
                break;

            Vector2Int ValidTile = AllValidTiles[Random.Range(0, AllValidTiles.Count - 1)];

            // check if tile is valid
            Vector2Int spawnPos;
            if (CanFitProp(inProp, ValidTile, inTileOccupancyData, out spawnPos))
            {
                Vector3 spawnPosFull = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
                GameObject prop = Instantiate(inProp.Prop, spawnPosFull, Quaternion.identity, inRoom.transform);
                if (inProp.IsASpawnpoint)
                {
                    inRoom.PotentialSpawnPoints.Add(prop.transform);
                }
                for (int c = 0; c < inProp.TileSize.x; c++)
                {
                    for (int r = 0; r < inProp.TileSize.y; r++)
                    {
                        Vector2Int tilePos = new Vector2Int(spawnPos.x + c, spawnPos.y + r);
                        if (inTileOccupancyData.ContainsKey(tilePos))
                        {
                            inTileOccupancyData[tilePos] = TileOccupancy.PROP;
                        }
                    }
                }
                placedProp = true;
                return;
            }
        }

        Debug.LogWarning("Could not place prop!");

    }

    private List<Vector2Int> GetAllTilesNearWalls(Dictionary<Vector2Int, TileOccupancy> inFullTileMap)
    {
        List<Vector2Int> tilesNearWalls = new List<Vector2Int>();

        HashSet<Vector2Int> allAvailableRoomTiles = new HashSet<Vector2Int>(inFullTileMap.Keys);

        allAvailableRoomTiles.UnionWith(AllCorridorPositions);

        foreach (var tile in inFullTileMap)
        {
            if (tile.Value == TileOccupancy.EMPTY) // only check valid (open space) tiles
            {
                Vector2Int tilePos = tile.Key;

                // Check all eight neighboring tiles
                Vector2Int[] neighbors = new Vector2Int[]
                {
                    new Vector2Int(tilePos.x + 1, tilePos.y),     // right
                    new Vector2Int(tilePos.x - 1, tilePos.y),     // left
                    new Vector2Int(tilePos.x, tilePos.y + 1),     // up
                    new Vector2Int(tilePos.x, tilePos.y - 1),     // down
                    new Vector2Int(tilePos.x + 1, tilePos.y + 1), // top-right
                    new Vector2Int(tilePos.x - 1, tilePos.y + 1), // top-left
                    new Vector2Int(tilePos.x + 1, tilePos.y - 1), // bottom-right
                    new Vector2Int(tilePos.x - 1, tilePos.y - 1)  // bottom-left
                };

                int neighborWallCount = 0;
                foreach (Vector2Int neighbor in neighbors)
                {
                    // if neighbour is out of bounds on all tiles (including corridors) or is in bounds but marked as wall
                    if ((!inFullTileMap.ContainsKey(neighbor) && !allAvailableRoomTiles.Contains(neighbor)) 
                        || inFullTileMap.ContainsKey(neighbor) && inFullTileMap[neighbor] == TileOccupancy.WALL) // if neighbor is a wall
                    {
                        neighborWallCount++;
                        if (neighborWallCount > 2)
                        {
                            tilesNearWalls.Add(tilePos); // add current valid tile to the list
                            break; // no need to check other neighbors
                        }
                    }
                }
            }
        }



        return tilesNearWalls;
    }

    private List<Vector2Int> GetAllTilesOpenSpace(Dictionary<Vector2Int, TileOccupancy> inFullTileMap)
    {
        List<Vector2Int> tilesInOpenSpace = new List<Vector2Int>();

        HashSet<Vector2Int> allAvailableRoomTiles = new HashSet<Vector2Int>(inFullTileMap.Keys);
        allAvailableRoomTiles.UnionWith(AllCorridorPositions);

        foreach (var tile in inFullTileMap)
        {
            if (tile.Value == TileOccupancy.EMPTY) // only check valid (open space) tiles
            {
                Vector2Int tilePos = tile.Key;

                // Check all eight neighboring tiles
                Vector2Int[] neighbors = new Vector2Int[]
                {
                    new Vector2Int(tilePos.x + 1, tilePos.y),     // right
                    new Vector2Int(tilePos.x - 1, tilePos.y),     // left
                    new Vector2Int(tilePos.x, tilePos.y + 1),     // up
                    new Vector2Int(tilePos.x, tilePos.y - 1),     // down
                    new Vector2Int(tilePos.x + 1, tilePos.y + 1), // top-right
                    new Vector2Int(tilePos.x - 1, tilePos.y + 1), // top-left
                    new Vector2Int(tilePos.x + 1, tilePos.y - 1), // bottom-right
                    new Vector2Int(tilePos.x - 1, tilePos.y - 1)  // bottom-left
                };

                bool invalid = false;
                foreach (Vector2Int neighbor in neighbors)
                {
                    if ((!inFullTileMap.ContainsKey(neighbor) && !allAvailableRoomTiles.Contains(neighbor)) 
                        || inFullTileMap.ContainsKey(neighbor) && inFullTileMap[neighbor] != TileOccupancy.EMPTY) // if neighbor is a wall
                    {
                        invalid = true; //invalid tile, just move on to the next tile
                        break; // no need to check other neighbors
                    }
                }
                if (!invalid)
                {
                    tilesInOpenSpace.Add(tilePos);
                }
            }
        }

        return tilesInOpenSpace;
    }

    private bool CanFitProp(PropPrefab inProp, Vector2Int inPotentialSpawnPos, Dictionary<Vector2Int, TileOccupancy> inTileOccupancyData, out Vector2Int inActualSpawnPos)
    {
        inActualSpawnPos = inPotentialSpawnPos;
        Vector2Int propBounds = inProp.TileSize;
        int prop_cols = propBounds.x;
        int prop_rows = propBounds.y;

        if (inProp.RequiredSpacing != 0)
        {
            prop_cols += 2 * inProp.RequiredSpacing;
            prop_rows += 2 * inProp.RequiredSpacing;
        }

        int totalTilesToCheck = prop_rows * prop_cols;

        for (int i = 0; i < totalTilesToCheck; i++)
        {
            Vector2Int startingPos = new Vector2Int(inPotentialSpawnPos.x + (i % prop_cols), inPotentialSpawnPos.y - (i / prop_rows));
            bool foundFittingSpot = true;
            for (int r = 0; r < prop_rows; r++)
            {
                for (int c = 0; c < prop_cols; c++)
                {
                    Vector2Int tileToCheck = new Vector2Int(startingPos.x + c, startingPos.y + r);
                    if (!inTileOccupancyData.ContainsKey(tileToCheck) || inTileOccupancyData[tileToCheck] != TileOccupancy.EMPTY)
                        foundFittingSpot = false;
                }
            }
            if (foundFittingSpot)
            {
                inActualSpawnPos = new Vector2Int(startingPos.x + inProp.RequiredSpacing, startingPos.y + inProp.RequiredSpacing);
                if (!inTileOccupancyData.ContainsKey(inActualSpawnPos))
                    return false;

                return true;
            }
        }

        return false;
    }
}
