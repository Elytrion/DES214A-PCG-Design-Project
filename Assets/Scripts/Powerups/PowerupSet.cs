using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PowerupSet", menuName = "Powerups/PowerupSet")]
public class PowerupSet : ScriptableObject
{
    [System.Serializable]
    public struct PowerupContainer
    {
        public PowerUp PowerupType;
        public Sprite PowerupSprite;
        public float SpawnWeight;
    };

    public List<PowerupContainer> AllPowerups;
    
    public GameObject BasePowerupPrefab;

    public GameObject SpawnRandomPowerUp(Vector3 inSpawnPos)
    {
        PowerupContainer randomPowerupContainer = GetRandomPowerup();
        PowerUp randomPowerup = randomPowerupContainer.PowerupType;
        Sprite powerupSprite = randomPowerupContainer.PowerupSprite;
        
        GameObject newPowerup = Instantiate(BasePowerupPrefab, inSpawnPos, Quaternion.identity);
        var pup = newPowerup.GetComponent<PowerUpPickup>();    
        pup.PowerUpLogic = randomPowerup;
        pup.InstantPickup = false;
        pup.OnlyPlayerPickup = true;
        pup.GetComponent<PickupPrompt>().PromptText.text = "Press E To Collect";
        pup.Prompt.SetActive(false);
        pup.CanBePickedUp = true;
        pup.RequiresCost = false;
        
        newPowerup.GetComponent<SpriteRenderer>().sprite = powerupSprite;
        return newPowerup;
    }

    public PowerupContainer GetRandomPowerup()
    {
        float totalWeight = 0;
        foreach (PowerupContainer pc in AllPowerups)
        {
            totalWeight += pc.SpawnWeight;
        }

        float randomWeight = Random.Range(0, totalWeight);
        float currentWeight = 0;
        foreach (PowerupContainer pc in AllPowerups)
        {
            currentWeight += pc.SpawnWeight;
            if (currentWeight >= randomWeight)
            {
                return pc;
            }
        }

        return AllPowerups[0];
    }

}
