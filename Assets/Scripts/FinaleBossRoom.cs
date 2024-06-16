using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIR = Direction2D.DIRECTION;

public class FinaleBossRoom : MonoBehaviour
{
    private BossFightManager _bfm;

    public RoomSetter _roomSetter;
    public GameObject _blockingWallsNorth;
    public GameObject _blockingWallsSouth;
    public GameObject _blockingWallsEast;
    public GameObject _blockingWallsWest;
    public GameObject _bossShield;
    // Start is called before the first frame update
    void Start()
    {
        _bfm = FindObjectOfType<BossFightManager>();
        OpenAll();
        _bossShield.SetActive(true);
        _bfm.FinaleRoomObjects = this;
    }

    public void ShutEntrances()
    {
        switch (_roomSetter.EntranceDirection)
        {
            case DIR.NORTH:
                _blockingWallsNorth.SetActive(true);
                break;
            case DIR.SOUTH:
                _blockingWallsSouth.SetActive(true);
                break;
            case DIR.EAST:
                _blockingWallsEast.SetActive(true);
                break;
            case DIR.WEST:
                _blockingWallsWest.SetActive(true);
                break;
        };
    }

    public void OpenEntrances()
    {
        switch (_roomSetter.EntranceDirection)
        {
            case DIR.NORTH:
                _blockingWallsNorth.SetActive(false);
                break;
            case DIR.SOUTH:
                _blockingWallsSouth.SetActive(false);
                break;
            case DIR.EAST:
                _blockingWallsEast.SetActive(false);
                break;
            case DIR.WEST:
                _blockingWallsWest.SetActive(false);
                break;
        };
    }

    public void ShutAll()
    {
        _blockingWallsNorth.SetActive(true);
        _blockingWallsSouth.SetActive(true);
        _blockingWallsEast.SetActive(true);
        _blockingWallsWest.SetActive(true);
    }

    public void OpenAll()
    {
        _blockingWallsNorth.SetActive(false);
        _blockingWallsSouth.SetActive(false);
        _blockingWallsEast.SetActive(false);
        _blockingWallsWest.SetActive(false);
    }

    public void ShutAllButEntrance()
    {
        switch (_roomSetter.EntranceDirection)
        {
            case DIR.NORTH:
                _blockingWallsNorth.SetActive(false);
                _blockingWallsSouth.SetActive(true);
                _blockingWallsEast.SetActive(true);
                _blockingWallsWest.SetActive(true);
                break;
            case DIR.SOUTH:
                _blockingWallsNorth.SetActive(true);
                _blockingWallsSouth.SetActive(false);
                _blockingWallsEast.SetActive(true);
                _blockingWallsWest.SetActive(true);
                break;
            case DIR.EAST:
                _blockingWallsNorth.SetActive(true);
                _blockingWallsSouth.SetActive(true);
                _blockingWallsEast.SetActive(false);
                _blockingWallsWest.SetActive(true);
                break;
            case DIR.WEST:
                _blockingWallsNorth.SetActive(true);
                _blockingWallsSouth.SetActive(true);
                _blockingWallsEast.SetActive(true);
                _blockingWallsWest.SetActive(false);
                break;
        };
    }

}
