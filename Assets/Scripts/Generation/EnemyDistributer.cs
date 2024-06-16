using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGraphNode = MapGraph.MapGraphNode;
using DIR = Direction2D.DIRECTION;
using System.Linq;

public class EnemyDistributer : MonoBehaviour
{
    [SerializeField]
    private MapGenerator _mapGenerator;
    
    public EnemyGenerator EnemyPool;

    public string[] IgnoredIDs;
    public int NumOfIgnoredStarts;
    public int NumOfAllIgnored;

    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    public List<EnemyBase> SpawnedEnemiesScripts = new List<EnemyBase>();

    [HideInInspector]
    public int NumOfMaps;
    [HideInInspector]
    public Vector3 DifficultyThresholdRatios = new Vector3(0.3f, 0.6f, 1f);

    public int BaseHealth = 3;

    public int SumOfRooms;

    public void DistributeEnemies(MapGraph inMGraph, Dictionary<string, GameObject> inSpawnedRoomsPerID, int inTotalRoomCount)
    {
        // remove previous enemies and reset lists
        foreach (GameObject go in _spawnedEnemies)
        {
            Destroy(go);
        }
        _spawnedEnemies = new List<GameObject>();
        SpawnedEnemiesScripts = new List<EnemyBase>();

        Queue<MapGraphNode> queue = new Queue<MapGraphNode>();
        HashSet<MapGraphNode> visited = new HashSet<MapGraphNode>();
        Dictionary<string, int> distanceFromSpawn = new Dictionary<string, int>();
        MapGraphNode root = inMGraph.GetRootNode();

        queue.Enqueue(root);
        visited.Add(root);
        distanceFromSpawn.Add(root.ID, 0);
        while (queue.Count > 0)
        {
            MapGraphNode parent = queue.Dequeue();

            if (!IgnoredIDs.Contains(parent.ID))
            {
                SpawnEnemies(inSpawnedRoomsPerID[parent.ID].GetComponent<RoomSetter>(), distanceFromSpawn[parent.ID], int.Parse(parent.ID), inTotalRoomCount - NumOfAllIgnored);
            }

            foreach (string childID in parent.Neighbors)
            {
                MapGraphNode child = inMGraph.GetNode(childID);
                if (!visited.Contains(child) && !queue.Contains(child))
                {
                    queue.Enqueue(child);
                    visited.Add(child);
                    distanceFromSpawn.Add(child.ID, distanceFromSpawn[parent.ID] + 1);
                }
            }
        }

        SumOfRooms += inTotalRoomCount;
    }

    public void SpawnEnemy(Vector3 inPosition, Vector3 inSpawnerPosition, List<EnemyAttack> inDoNotSpawnTypes = null)
    {
        int inTotalRoomCount = _mapGenerator.MapGraphChosen.GetTotalRoomCount();

        // Get room id from spawner position
        string roomID = _mapGenerator.GetIDFromPosition(inSpawnerPosition);

        // Get distance from spawn
        MapGraphNode root = _mapGenerator.MapGraphChosen.GetRootNode();
        int distanceFromSpawn = _mapGenerator.MapGraphChosen.GetNodeDistanceBetweenNodes(root.ID, roomID);

        GameObject enemyToSpawn = SpawnRandomEnemyToSpawnBasedOnDifficulty(inPosition, distanceFromSpawn, int.Parse(roomID), inTotalRoomCount, inDoNotSpawnTypes);
        SpawnedEnemiesScripts.Add(enemyToSpawn.GetComponent<EnemyBase>());
        _spawnedEnemies.Add(enemyToSpawn);

        // Add to room setter
        var roomSetter = _mapGenerator.GetRoomFromID(roomID).GetComponent<RoomSetter>();
        if (roomSetter != null)
        {
            roomSetter.Enemies.Add(enemyToSpawn.GetComponent<EnemyBase>());
        }

    }

    public void SpawnEnemies(RoomSetter inRoom, int inDistanceFromSpawn, int inRoomNum, int inTotalRoomCount)
    {
        List<Transform> allSpawnPoints = inRoom.PotentialSpawnPoints;

        int totalEnemiesInRoom = Mathf.Min(inRoom.PotentialSpawnPoints.Count, inDistanceFromSpawn);

        // Spawn enemies
        for (int i = 0; i < totalEnemiesInRoom; i++)
        {
            int randomIndex = Random.Range(0, allSpawnPoints.Count);
            Transform spawnPoint = allSpawnPoints[randomIndex];
            allSpawnPoints.RemoveAt(randomIndex);

            GameObject enemyToSpawn = SpawnRandomEnemyToSpawnBasedOnDifficulty(spawnPoint.position, inDistanceFromSpawn, inRoomNum, inTotalRoomCount);
            SpawnedEnemiesScripts.Add(enemyToSpawn.GetComponent<EnemyBase>());
            _spawnedEnemies.Add(enemyToSpawn);
        }
    }

    private GameObject SpawnRandomEnemyToSpawnBasedOnDifficulty(Vector3 spawnPoint, int inDistanceFromSpawn, int inRoomNum, int inTotalRoomCount, List<EnemyAttack> inDoNotSpawnTypes = null)
    {
        Dictionary<EnemyGenerator.EnemyDifficulty, float> weightedDifficulties = SetupDifficulty(inRoomNum, inTotalRoomCount);

        EnemyGenerator.EnemyDifficulty enemyDifficulty = PCG.WeightedRandom(weightedDifficulties);

        int enemyHealth = BaseHealth * (int)enemyDifficulty + (NumOfMaps + inDistanceFromSpawn)/5;

        return EnemyPool.CreateEnemy(spawnPoint, enemyDifficulty, enemyHealth, inDoNotSpawnTypes);
    }

    private Dictionary<EnemyGenerator.EnemyDifficulty, float> SetupDifficulty(int currentRoom, int totalRoomsInCurrentGroup)
    {
        var enemyDifficultyWeighted = new Dictionary<EnemyGenerator.EnemyDifficulty, float>();
        // Calculate total progression
        float totalProgression = (float)(SumOfRooms + (currentRoom - NumOfIgnoredStarts)) / (SumOfRooms + totalRoomsInCurrentGroup);
        float enemyAtoBThreshold = DifficultyThresholdRatios.x;
        float enemyAtoBandBtoCThreshold = DifficultyThresholdRatios.y;
        float enemyBtoCThreshold = DifficultyThresholdRatios.z;

        float enemyARatio, enemyBRatio, enemyCRatio;

        if (totalProgression <= enemyAtoBThreshold)
        {
            enemyARatio = 1 - (totalProgression / enemyAtoBThreshold);
            enemyBRatio = totalProgression / enemyAtoBThreshold;
            enemyCRatio = 0;
        }
        else if (totalProgression <= enemyAtoBandBtoCThreshold)
        {
            enemyARatio = 0.3f;
            enemyBRatio = 0.6f;
            enemyCRatio = 0.1f;
        }
        else // totalProgression <= enemyBtoCThreshold
        {
            enemyARatio = 0;
            enemyBRatio = 1 - ((totalProgression - enemyAtoBandBtoCThreshold) / (enemyBtoCThreshold - enemyAtoBandBtoCThreshold));
            enemyCRatio = (totalProgression - enemyAtoBandBtoCThreshold) / (enemyBtoCThreshold - enemyAtoBandBtoCThreshold);
        }

        enemyDifficultyWeighted.Add(EnemyGenerator.EnemyDifficulty.EASY, enemyARatio);
        enemyDifficultyWeighted.Add(EnemyGenerator.EnemyDifficulty.MEDIUM, enemyBRatio);
        enemyDifficultyWeighted.Add(EnemyGenerator.EnemyDifficulty.HARD, enemyCRatio);

        return enemyDifficultyWeighted;
    }
}
