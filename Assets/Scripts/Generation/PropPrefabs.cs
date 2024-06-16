using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropPrefab = PropPlacer.PropPrefab;

[CreateAssetMenu(fileName = "New Prop Set", menuName = "Prop Set")]
public class PropPrefabs : ScriptableObject
{
    [System.Serializable]
    public struct PropType
    {
        public PropPrefab PropData;
        public int SpawnCount;
    };

    public List<PropType> AllPropTypes;
}
