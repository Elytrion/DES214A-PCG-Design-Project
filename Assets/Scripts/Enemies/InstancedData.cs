using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Blackboard
public class InstancedData : MonoBehaviour
{
    private Dictionary<string, object> _data = new Dictionary<string, object>();

    public T GetID<T>(string id)
    {
        if (_data.TryGetValue(id, out object value))
        {
            if (value is T t)
            {
                return t;
            }
            else
            {
                Debug.LogError($"Value for ID {id} is not of type {typeof(T)}");
            }
        }
        return default(T);
    }

    public void SetID<T>(string id, T value)
    {
        if (!_data.ContainsKey(id))
        {
            _data.Add(id, value);
        }
        else
        {
            _data[id] = value;
        }
    }
}
