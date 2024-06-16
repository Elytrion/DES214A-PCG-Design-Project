using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowProp : MonoBehaviour
{
    [System.Serializable]
    public struct Prop
    {
        public Sprite PropTypeToUse;
        public float Weight;
    }

    public Prop[] Props;

    private SpriteRenderer _sr;

    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (_sr == null)
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);

        _sr.sprite = GetRandomProp();
    }

    private Sprite GetRandomProp()
    {
        float totalSpawnRate = 0.0f;
        foreach (var prop in Props)
        {
            totalSpawnRate += prop.Weight;
        }

        float randomValue = Random.value;
        float currentSpawnRate = 0.0f;
        foreach (var prop in Props)
        {
            currentSpawnRate += prop.Weight;
            if (randomValue <= currentSpawnRate / totalSpawnRate)
                return prop.PropTypeToUse;
        }

        return null;
    }
}
