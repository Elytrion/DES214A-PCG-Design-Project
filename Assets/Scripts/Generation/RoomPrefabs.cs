using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Room Set", menuName = "RoomSet")]
public class RoomPrefabs : ScriptableObject
{
    [System.Serializable]
    public struct RoomModuleHolder
    {
        public GameObject RoomPrefab;
        public RoomModule RoomModule;
    }

    [System.Serializable]
    public class RoomPrefab
    {
        public string TypeID;
        public List<RoomModuleHolder> AllRoomsOfType;
    }

    public List<RoomPrefab> AllRoomTypes;

    public List<RoomModuleHolder> AllExtraRooms;

    public RoomPrefab GetRoomPrefabByType(string TypeID)
    {
        return AllRoomTypes.Find(x => x.TypeID == TypeID);
    }

    public RoomModuleHolder GetRandomRoomModuleByType(string TypeID)
    {
        RoomPrefab prefab = GetRoomPrefabByType(TypeID);
        return prefab.AllRoomsOfType[Random.Range(0, prefab.AllRoomsOfType.Count)];
    }

    public RoomModuleHolder GetRandomExtraRoomModule()
    {
        return AllExtraRooms[Random.Range(0, AllExtraRooms.Count)];
    }
}
