using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public PowerupSet PowerupSetUsed;
    [Range(0.0f, 100.0f)]
    public float SpawnRate = 5.0f;

    [HideInInspector]
    public List<GameObject> spawnedItems;

    public GameObject CoinPrefab;
    public GameObject DebrisParticleEffect;
    public GameObject Debris;
    
    private void Start()
    {
        spawnedItems = new List<GameObject>();
        DebrisParticleEffect.SetActive(false);
    }

    public void TrySpawnPowerup(Vector3 inSpawnPos, float usedSpawnRate = -1.0f)
    {
        float spawnRate = usedSpawnRate > 0.0f ? usedSpawnRate : SpawnRate;
        float chance = Random.value; 
        if (chance <= spawnRate / 100)
        {
            var powerup = PowerupSetUsed.SpawnRandomPowerUp(inSpawnPos);
            spawnedItems.Add(powerup);
        }
    }

    public void TrySpawnCoin(Vector3 inSpawnPos, int inValue = 1)
    {
        var coin = Instantiate(CoinPrefab, inSpawnPos, Quaternion.identity);
        coin.GetComponent<CoinPickup>().CoinValue = inValue;
        spawnedItems.Add(coin);
    }

    public void TrySpawnDebris(Vector3 inSpawnPos, bool inSpawnDebris)
    {
        if (inSpawnDebris)
        {
            var debris = Instantiate(Debris, inSpawnPos, Quaternion.identity);
            spawnedItems.Add(debris);
        }
        
        DebrisParticleEffect.SetActive(true);
        DebrisParticleEffect.transform.position = inSpawnPos;
        DebrisParticleEffect.GetComponent<ParticleSystem>().Play();     

    }
}
