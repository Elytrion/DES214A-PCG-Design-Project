using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGraphNode = MapGraph.MapGraphNode;
using DIR = Direction2D.DIRECTION;
using System.Linq;

public class MapGenerator : AbstractDunGen
{
    public class RoomHolder
    {
        public string ID;
        public GameObject RoomToSpawn;
        public RoomModule RoomModuleData;
        public Vector2Int SpawnPosition;
        public RoomHolder(GameObject inRTS = null, RoomModule inRM = null, Vector2Int inSP = new Vector2Int())
        {
            RoomToSpawn = inRTS; RoomModuleData = inRM; SpawnPosition = inSP; ID = null;
        }
    }

    [System.Serializable]
    public struct RoomPrefabTileSet
    {
        public RoomPrefabs RoomPrefabSet;
        public TileBaseSet RoomFloorsTileSet;
        public TileBaseSet RoomWallsTileSet;
    }


    [SerializeField]
    [Range(0, 100)]
    private int _offset = 1;
    [SerializeField]
    private float _cellSize = 1.0f;
    [SerializeField]
    private int _corridorSize = 1;
    public List<MapGraph> _mapGraphPool;
    public List<RoomPrefabTileSet> _roomPrefabPools;
    [HideInInspector]
    public RoomPrefabs _selectedRoomPrefabPool;
    [SerializeField]
    private static Vector2Int BannedDirection;
    
    // all instantiated rooms
    public Dictionary<string, RoomHolder> _instantiatedRooms = new Dictionary<string, RoomHolder>();
    private Dictionary<string, GameObject> _sgoDict = new Dictionary<string, GameObject>();
    private List<GameObject> _spawnedGameObjects = new List<GameObject>();

    public MapGraph MapGraphChosen;
    [SerializeField]
    private EnemyDistributer _enemySpawner;
    [SerializeField]
    private PropPlacer _propSpawner;
    [SerializeField]
    private PowerUpSpawner _powerupSpawner;

    #region Getters
    public Vector3 GetStartingPosition(string ID = null)
    {
        if (ID == null)
            return _spawnedGameObjects[0].transform.position;

        Vector2Int pos = _instantiatedRooms[ID].SpawnPosition;
        return new Vector3(pos.x, pos.y, 0);
    }
    public Vector3 GetEndPosition(string ID = null)
    {
        if (ID == null)
            return _spawnedGameObjects[_spawnedGameObjects.Count - 1].transform.position;

        Vector2Int pos = _instantiatedRooms[ID].SpawnPosition;
        return new Vector3(pos.x, pos.y, 0);
    }
    public string GetIDFromPosition(Vector3 inPos)
    {
        Vector2Int cellPos = CellPosFromWorldPos(inPos);
        foreach (KeyValuePair<string, RoomHolder> kvp in _instantiatedRooms)
        {
            var roomHolder = kvp.Value;
            if (roomHolder != null)
            {
                Vector2Int sizeOfRoom = roomHolder.RoomModuleData.GetGridSize();
                Bounds roomBounds = new Bounds(new Vector3(roomHolder.SpawnPosition.x, roomHolder.SpawnPosition.y, 0), new Vector3(sizeOfRoom.x, sizeOfRoom.y, 0));
                if (roomBounds.Contains(new Vector3(cellPos.x, cellPos.y, 0)))
                    return roomHolder.ID;
            }
        }
        return null;
    }
    public GameObject GetRoomFromID(string ID)
    {
        if (_sgoDict.ContainsKey(ID))
            return _sgoDict[ID];
        return null;
    }
    #endregion


    protected override void RunGeneration()
    {
        ResetMap(); // remove prev data if any

        MapGraph mgraphInUse = _mapGraphPool[Random.Range(0, _mapGraphPool.Count)];
        RoomPrefabTileSet _selectRoomSet = _roomPrefabPools[Random.Range(0, _roomPrefabPools.Count)];
        _selectedRoomPrefabPool = _selectRoomSet.RoomPrefabSet;
        _tileMapVis.FloorTileBaseSet = _selectRoomSet.RoomFloorsTileSet;
        _tileMapVis.WallTileBaseSet = _selectRoomSet.RoomWallsTileSet;
        MapGraphChosen = mgraphInUse;
        GenerateRooms(mgraphInUse);
        if (GenerationFailed)
        {
            Debug.LogWarning("Generation Failed (Rooms), retrying...");
            GenerateMap();
            return;
        }
        HashSet<Vector2Int> AllFloorTiles = new HashSet<Vector2Int>();
        foreach (RoomHolder rh in _instantiatedRooms.Values)
        {
            Vector3 worldPos = WorldPosFromCellPos(rh.SpawnPosition);
            GameObject room = Instantiate(rh.RoomToSpawn, worldPos, Quaternion.identity);
            if (_sgoDict.ContainsKey(rh.ID))
                _sgoDict[rh.ID] = room;
            else
                _sgoDict.Add(rh.ID, room);
            _spawnedGameObjects.Add(room);
            var roomModule = room.GetComponent<RoomModule>();
            if (roomModule != null)
            {
                roomModule.ID = rh.ID;
                //Vector2Int spawnPos = new Vector2Int(rh.SpawnPosition.x - roomModule.GetGridSize().x / 2, rh.SpawnPosition.y - roomModule.GetGridSize().y / 2);
                AllFloorTiles.UnionWith(roomModule.GetAllTilePositions(rh.SpawnPosition));
            }
        }
        HashSet<Vector2Int> corridorTilesPos = GenerateCorridors(mgraphInUse);
        if (GenerationFailed)
        {
            Debug.LogWarning("Generation Failed (Corridors), retrying...");
            GenerateMap();
            return;
        }
        _tileMapVis.PaintFloorTiles(corridorTilesPos);

        AllFloorTiles.UnionWith(corridorTilesPos);

        CreateWalls(AllFloorTiles, _tileMapVis);
        if (_propSpawner != null)
        {
            _propSpawner.PlacePropsInAllRooms(_spawnedGameObjects, corridorTilesPos);
        }

        if (_enemySpawner != null)
        {
            _enemySpawner.DistributeEnemies(mgraphInUse, _sgoDict, _instantiatedRooms.Count);
        }
    }

    #region Helper Functions
    private Vector3 WorldPosFromCellPos(Vector2Int inGridPos)
    {
        Vector3 cell_world_size = new Vector3(_cellSize, _cellSize, 0);
        return Vector3.Scale(new Vector3(inGridPos.x, inGridPos.y, 0), cell_world_size);
    }
    private Vector2Int CellPosFromWorldPos(Vector3 inWorldPos)
    {
        int x = Mathf.FloorToInt(inWorldPos.x / _cellSize);
        int y = Mathf.FloorToInt(inWorldPos.y / _cellSize);

        return new Vector2Int(x, y);
    }
    private bool IsBoundsOverlapping(BoundsInt a, BoundsInt b)
    {
        Vector3Int aMax = a.max, aMin = a.min;
        Vector3Int bMax = b.max, bMin = b.min;
        return !(aMax.x < bMin.x || aMin.x > bMax.x || aMax.y < bMin.y || aMin.y > bMax.y);
    }
    private bool IsRoomOverlappingWithSpawnedRooms(Vector2Int inSpawnPoint, Vector2Int inRoomSize)
    {
        BoundsInt rooma_bounds = new BoundsInt(new Vector3Int(inSpawnPoint.x, inSpawnPoint.y, 0), new Vector3Int(inRoomSize.x, inRoomSize.y, 0));
        foreach (RoomHolder roomb in _instantiatedRooms.Values)
        {
            BoundsInt roomb_bounds = new BoundsInt(
                new Vector3Int(roomb.SpawnPosition.x, roomb.SpawnPosition.y, 0), 
                new Vector3Int(roomb.RoomModuleData.GetGridSize().x, roomb.RoomModuleData.GetGridSize().y, 0));
            if (IsBoundsOverlapping(rooma_bounds, roomb_bounds))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Room Generation
    private void GenerateRooms(MapGraph inMGraph)
    {
        Queue<MapGraphNode> queue = new Queue<MapGraphNode>();
        HashSet<MapGraphNode> visited = new HashSet<MapGraphNode>();
        MapGraphNode root = inMGraph.GetRootNode();
        Vector2Int bannedDir = Direction2D.GetRandomCardinalDirection();
        BannedDirection = bannedDir;
        
        queue.Enqueue(root);
        visited.Add(root);
        while (queue.Count > 0)
        {
            MapGraphNode parent = queue.Dequeue();
            var room = CreateRoom(parent, bannedDir, inMGraph);

            if (room.RoomModuleData == null || room.RoomToSpawn == null)
            {
                Debug.LogWarning("BFS Failed!");
                GenerationFailed = true;
                break;
            }

            _instantiatedRooms.Add(parent.ID, room);

            foreach (string childID in parent.Neighbors)
            {
                MapGraphNode child = inMGraph.GetNode(childID);
                if (!visited.Contains(child) && !queue.Contains(child))
                {
                    queue.Enqueue(child);
                    visited.Add(child);
                }
            }
        }
    }

    private RoomHolder CreateRoom(MapGraphNode inNode, Vector2Int bannedDir, MapGraph inMGraph)
    {      
        // ban a direction (do not choose this direction to spawn rooms in)
        RoomHolder spawnedRoomHolder;
        if (!TrySpawnRoom(inNode, bannedDir, inMGraph ,out spawnedRoomHolder))
        {
            Debug.LogWarning("Could not spawn room!");
            return new RoomHolder();
        }

        return spawnedRoomHolder;
    }

    private bool TrySpawnRoom(MapGraphNode inNode, Vector2Int inBanDir, MapGraph inMGraph, out RoomHolder outRoomHolder)
    {
        bool isPlaced = false;
        int max_attempts = 128;
        int curr_attempts = 0;
        outRoomHolder = new RoomHolder();

        // get a random room module of type
        RoomPrefabs.RoomModuleHolder rmHolder = _selectedRoomPrefabPool.GetRandomRoomModuleByType(inNode.ID);      

        List<RoomHolder> availableRoomConnections = new List<RoomHolder>();
        foreach (RoomHolder spawnedRoom in _instantiatedRooms.Values)
        {
            availableRoomConnections.Add(spawnedRoom);
        }
        Vector2Int prevSpawnPt = DeterminePrevSpawnPt(inNode, availableRoomConnections);

        while (!isPlaced && curr_attempts < max_attempts)
        {
            curr_attempts++;
            var bannedDirs = GetBannedDirections(inNode, inBanDir, prevSpawnPt, availableRoomConnections, inMGraph);
            Vector2Int cardinalDirection = Direction2D.GetRandomCardinalDirection(bannedDirs);
            var prevRoom = DeterminePrevRoomHolder(inNode, availableRoomConnections);
            Vector2Int prevRoomSize = new Vector2Int();
            if (prevRoom != null)
            {
                prevRoomSize = prevRoom.RoomModuleData.GetGridSize();
                float half_x = prevRoomSize.x * 0.5f;
                float half_y = prevRoomSize.y * 0.5f;
                prevRoomSize = new Vector2Int(Mathf.RoundToInt(half_x), Mathf.RoundToInt(half_y));
            }
            if (rmHolder.RoomModule == null)
            {
                Debug.LogWarning("Could not find room module!");
                return false;
            }

            int g_size = (cardinalDirection != Vector2Int.up && cardinalDirection != Vector2Int.down) ? (rmHolder.RoomModule.GetGridSize().x) : (rmHolder.RoomModule.GetGridSize().y);
            int prev_g_size = (cardinalDirection != Vector2Int.up && cardinalDirection != Vector2Int.down) ? (prevRoomSize.x) : (prevRoomSize.y);

            Vector2Int intialSpawnPos = prevSpawnPt + (cardinalDirection * (g_size + prev_g_size + _offset));
            if (!IsRoomOverlappingWithSpawnedRooms(intialSpawnPos, rmHolder.RoomModule.GetGridSize()))
            {
                isPlaced = true;
                outRoomHolder.ID = inNode.ID;
                outRoomHolder.RoomModuleData = rmHolder.RoomModule;
                outRoomHolder.RoomToSpawn = rmHolder.RoomPrefab;
                outRoomHolder.SpawnPosition = intialSpawnPos;
            }
        }
        
        return isPlaced;
    }

    private Vector2Int DeterminePrevSpawnPt(MapGraphNode inNode, List<RoomHolder> inSpawnedRooms)
    {
        if (inSpawnedRooms.Count == 0)
        {
            return _startPos;
        }

        return DeterminePrevRoomHolder(inNode, inSpawnedRooms).SpawnPosition;
    }

    private RoomHolder DeterminePrevRoomHolder(MapGraphNode inNode, List<RoomHolder> inSpawnedRooms)
    {
        if (inSpawnedRooms.Count == 0)
        {
            return null;
        }

        foreach (string ID in inNode.Neighbors)
        {
            var spawnedRoom = inSpawnedRooms.FirstOrDefault(rm => rm.ID == ID);
            if (spawnedRoom != null)
            {
                return spawnedRoom;
            }
        }

        return null;
    }

    private List<Vector2Int> GetBannedDirections(MapGraphNode inNode, Vector2Int inBannedDirection, Vector2Int inPrevSpawnPt, List<RoomHolder> inSpawnedRooms, MapGraph inMGraph)
    {
        List<Vector2Int> bannedDirs = new List<Vector2Int>();
        bannedDirs.Add(inBannedDirection);

        var neighborRooms = inSpawnedRooms.Where(d => inNode.Neighbors.Contains(d.ID)).ToList();

        if (neighborRooms.Count == 2) //TODO: HARD CODED
        {
            Vector2Int rooma_spt = neighborRooms[0].SpawnPosition;
            Vector2Int roomb_spt = neighborRooms[1].SpawnPosition;

            // check if horizontal, if so ban horizontal directions
            if (rooma_spt.x == roomb_spt.x)
            {
                bannedDirs.Add(Vector2Int.up);
                bannedDirs.Add(Vector2Int.down);
            }
            else if (rooma_spt.y == roomb_spt.y)
            {
                bannedDirs.Add(Vector2Int.right);
                bannedDirs.Add(Vector2Int.left);
            }
            else
            {
                // if neither, is on diagonal, ban the directions past the connecting room

                Vector2Int connectingRoom_spt = (inPrevSpawnPt == rooma_spt) ? rooma_spt : roomb_spt;
                Vector2Int otherRoom_spt = (inPrevSpawnPt == rooma_spt) ? roomb_spt : rooma_spt;

                Vector2Int intersection_a = new Vector2Int(connectingRoom_spt.x, otherRoom_spt.y);
                Vector2Int intersection_b = new Vector2Int(otherRoom_spt.x, connectingRoom_spt.y);

                // a - b to get b -> a
                Vector2 banDir_a = (Vector2)(connectingRoom_spt - intersection_a);
                Vector2 banDir_b = (Vector2)(connectingRoom_spt - intersection_b);

                banDir_a = banDir_a.normalized;
                banDir_b = banDir_b.normalized;

                Vector2Int banDira = Vector2Int.RoundToInt(banDir_a);
                Vector2Int banDirb = Vector2Int.RoundToInt(banDir_b);

                bannedDirs.Add(banDira);
                bannedDirs.Add(banDirb);
            }
        }


        return bannedDirs;
    }
    #endregion

    #region Corridor Generation
    private HashSet<Vector2Int> GenerateCorridors(MapGraph inMGraph)
    {
        HashSet<Vector2Int> corridorTiles = new HashSet<Vector2Int>();

        List<KeyValuePair<Vector2Int, Vector2Int>> connectedRooms = new List<KeyValuePair<Vector2Int, Vector2Int>>();

        foreach (MapGraphNode node in inMGraph.Nodes)
        {
            foreach (string neighborID in node.Neighbors)
            {
                RoomHolder node_room = _instantiatedRooms[node.ID];
                RoomHolder neighbor_room = _instantiatedRooms[neighborID];

                if (node_room == null || neighbor_room == null)
                {
                    GenerationFailed = true;
                    break;
                }

                KeyValuePair<Vector2Int, Vector2Int> roompair_1 = new KeyValuePair<Vector2Int, Vector2Int>(node_room.SpawnPosition, neighbor_room.SpawnPosition);
                KeyValuePair<Vector2Int, Vector2Int> roompair_2 = new KeyValuePair<Vector2Int, Vector2Int>(neighbor_room.SpawnPosition, node_room.SpawnPosition);
                

                
                if (connectedRooms.Contains(roompair_1) || connectedRooms.Contains(roompair_2))
                    continue;

                HashSet<Vector2Int> corridor = CreateCorridor(node_room.SpawnPosition, neighbor_room.SpawnPosition, _corridorSize);
                corridorTiles.UnionWith(corridor);
                connectedRooms.Add(roompair_1);
                SetConnections(node.ID, node_room.SpawnPosition, neighborID, neighbor_room.SpawnPosition);
            }
        }

        return corridorTiles;
    }
    

    public static HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination, int corridorWidth = 2)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        AddToCorridor(position, corridorWidth, corridor, true);

        if (BannedDirection.y == 0)
        {
            while (position.x != destination.x)
            {
                if (destination.x > position.x)
                {
                    position += Vector2Int.right;
                }
                else if (destination.x < position.x)
                {
                    position += Vector2Int.left;
                }
                AddToCorridor(position, corridorWidth, corridor, false);
            }
            while (position.y != destination.y)
            {
                if (destination.y > position.y)
                {
                    position += Vector2Int.up;
                }
                else if (destination.y < position.y)
                {
                    position += Vector2Int.down;
                }
                AddToCorridor(position, corridorWidth, corridor, true);
            }
        }
        else
        {
            while (position.y != destination.y)
            {
                if (destination.y > position.y)
                {
                    position += Vector2Int.up;
                }
                else if (destination.y < position.y)
                {
                    position += Vector2Int.down;
                }
                AddToCorridor(position, corridorWidth, corridor, true);
            }
            while (position.x != destination.x)
            {
                if (destination.x > position.x)
                {
                    position += Vector2Int.right;
                }
                else if (destination.x < position.x)
                {
                    position += Vector2Int.left;
                }
                AddToCorridor(position, corridorWidth, corridor, false);
            }
        }
        return corridor;
    }

    public static void AddToCorridor(Vector2Int position, int width, HashSet<Vector2Int> corridor, bool vertical)
    {
        for (int i = 0; i < width; i++)
        {
            if (vertical)
            {
                corridor.Add(position - new Vector2Int(i, 0));
            }
            else
            {
                corridor.Add(position + new Vector2Int(0, i));
            }
        }
    }

    public static Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    public void SetConnections(string ID_a, Vector2Int spt_a, string ID_b, Vector2Int spt_b)
    {
        RoomSetter room_a = _sgoDict[ID_a].GetComponent<RoomSetter>();
        RoomSetter room_b = _sgoDict[ID_b].GetComponent<RoomSetter>();

        Vector2 dir_exit_unnorm = (spt_b - spt_a);
        Vector2 dir_entrance_unnorm = (spt_a - spt_b);

        //if (spt_b.x != spt_a.x && spt_b.y != spt_a.x)
        //{
        //    if (BannedDirection.y == 0)
        //    {
        //        // create a new third point to connect the two points
        //        Vector2Int thirdPoint = new Vector2Int(spt_a.x, spt_b.y);
        //        dir_entrance_unnorm = (thirdPoint - spt_b);
        //        dir_exit_unnorm = (thirdPoint - spt_a);
        //    }
        //    else
        //    {
        //        Vector2Int thirdPoint = new Vector2Int(spt_b.x, spt_a.y);
        //        dir_entrance_unnorm = (thirdPoint - spt_b);
        //        dir_exit_unnorm = (thirdPoint - spt_a);
        //    }
        //}

        Vector2Int dir_exit = new Vector2Int(Mathf.RoundToInt(dir_exit_unnorm.normalized.x), Mathf.RoundToInt(dir_exit_unnorm.normalized.y));
        Vector2Int dir_entrance = new Vector2Int(Mathf.RoundToInt(dir_entrance_unnorm.normalized.x), Mathf.RoundToInt(dir_entrance_unnorm.normalized.y));

        DIR exitDir = Direction2D.DetermineDirection(dir_exit);
        DIR entranceDir = Direction2D.DetermineDirection(dir_entrance);

        room_a.ExitDirections.Add(exitDir);
        room_b.EntranceDirection = entranceDir;
    }

    #endregion

    #region Wall Generation
    private void CreateWalls(HashSet<Vector2Int> inFloorPositions, TileMapVisualizer inTMV)
    {
        var basicWallPositions = FindWallsInDirections(inFloorPositions, Direction2D.cardinalDirectionsList);
        inTMV.PaintWallTiles(basicWallPositions);
    }

    private Dictionary<Vector2Int, int> FindWallsInDirections(HashSet<Vector2Int> inFloorPositions, List<Vector2Int> inDirections)
    {
        Dictionary<Vector2Int, int> wallPositions = new Dictionary<Vector2Int, int>();

        foreach (var floorPos in inFloorPositions)
        {
            foreach (var dir in inDirections)
            {
                var posToCheck = floorPos + dir;
                if (!inFloorPositions.Contains(posToCheck))
                {
                    int label = 0;
                    foreach (var direction in inDirections)
                    {
                        if (inFloorPositions.Contains( posToCheck + direction ))
                            label |= Direction2D.DetermineDirectionLabel(direction);
                    }                
                    if (!wallPositions.ContainsKey(posToCheck))
                        wallPositions.Add(posToCheck, label);
                }
            }
        }

        return wallPositions;
    }
    #endregion

    public void ResetMap()
    {
        foreach (GameObject go in _spawnedGameObjects)
        {
            Destroy(go);
        }
        foreach (var spawnedItems in _powerupSpawner.spawnedItems)
        {
            Destroy(spawnedItems);
        }
        _powerupSpawner.spawnedItems.Clear();
        _spawnedGameObjects = new List<GameObject>();
        _instantiatedRooms = new Dictionary<string, RoomHolder>();
        BannedDirection = Vector2Int.zero;
        _sgoDict = new Dictionary<string, GameObject>();
    }
}
