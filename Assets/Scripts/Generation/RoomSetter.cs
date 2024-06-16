using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DIR = Direction2D.DIRECTION;
using UnityEngine.Tilemaps;

public class RoomSetter : MonoBehaviour
{
    public PropPrefabs Props;
    public bool CanHaveProps;

    [System.Serializable]
    public struct Setup
    {
        public DIR SetupDirection;
        public GameObject SetupObj;
    }

    public List<Setup> EntranceSetups;
    public List<Setup> ExitSetups;

    public DIR EntranceDirection = DIR.NONE;
    public List<DIR> ExitDirections;

    public List<Tilemap> AllTilemaps;

    public List<Transform> PotentialSpawnPoints;

    public List<EnemyBase> Enemies;

    public void Start()
    {
        foreach (Setup es in EntranceSetups)
        {
            es.SetupObj.SetActive(false);
        }

        foreach (Setup exs in ExitSetups)
        {
            exs.SetupObj.SetActive(false);
        }
    }

    public void SetupRoom()
    {
        SetupEntrance();
        SetupExit();
    }
    private void SetupEntrance()
    {
        if (EntranceSetups.Count <= 0)
            return;

        GameObject setupObj = EntranceSetups.FirstOrDefault(es => es.SetupDirection == EntranceDirection).SetupObj;
        if (setupObj != null)
        {
            setupObj.SetActive(true);
        }
    }
    private void SetupExit()
    {
        if (ExitSetups.Count <= 0)
            return;

        foreach (Setup exs in ExitSetups)
        {
            if (ExitDirections.Contains(exs.SetupDirection))
            {
                if (exs.SetupObj != null)
                {
                    exs.SetupObj.SetActive(false);
                }
            }
        }
    }

    
}
