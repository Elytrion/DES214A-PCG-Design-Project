using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectManager : MonoBehaviour
{
    [SerializeField]
    private Entity _self;
    public Dictionary<string, float> ActivePowerUps = new Dictionary<string, float>();
    public Dictionary<string, PowerUp> ActiveEffects = new Dictionary<string, PowerUp>();

    public List<PowerUp> activeEffects;

    private void Start()
    {
        if (_self == null)
            _self = GetComponent<Entity>();
    }

    private void Update()
    {
        List<string> keysToRemove = new List<string>();

        // Create a copy of the dictionary keys
        List<string> keys = new List<string>(ActivePowerUps.Keys);
        
        foreach (string key in keys)
        {
            if (ActivePowerUps[key] <= 0.0f)
            {
                keysToRemove.Add(key);
                continue;
            }
            ActivePowerUps[key] -= Time.deltaTime;
            if (ActiveEffects[key].overTime)
            {
                if (ActivePowerUps[key] % ActiveEffects[key].overTimeTick <= Time.deltaTime)
                {
                    ActiveEffects[key].ApplyEffect(_self);
                }
            }
        }

        foreach (string key in keysToRemove)
        {
            if (!ActiveEffects[key].isInstant)
            {
                ActiveEffects[key].RemoveEffect(_self);
            }
            ActivePowerUps.Remove(key);
            ActiveEffects.Remove(key);
        }

        activeEffects = new List<PowerUp>(ActiveEffects.Values);
    }

    public void RemovePowerup(PowerUp inPowerUp)
    {
        if (HasPowerup(inPowerUp))
        {
            ActivePowerUps[inPowerUp.PowerupID] = -0.01f;
        }
    }

    public bool HasPowerup(PowerUp inPowerup)
    {
        return ActivePowerUps.ContainsKey(inPowerup.PowerupID);
    }

    public void ApplyPowerUp(PowerUp inPowerUp)
    {
        if (_self.NoDamage)
            return;

        if (inPowerUp.Duration >= 0.0f)
        {
            if (ActivePowerUps.ContainsKey(inPowerUp.PowerupID))
            {
                ActivePowerUps[inPowerUp.PowerupID] = (inPowerUp.DurationStackable) ? (inPowerUp.Duration + ActivePowerUps[inPowerUp.PowerupID]) : inPowerUp.Duration;
                if (inPowerUp.EffectStackable)
                {
                    inPowerUp.ApplyEffect(_self);
                }
            }
            else
            {
                ActiveEffects.Add(inPowerUp.PowerupID, inPowerUp);
                ActivePowerUps.Add(inPowerUp.PowerupID, inPowerUp.Duration);
                inPowerUp.ApplyEffect(_self);
            }
        }
    }
    
    public void RemoveAllEffects()
    {
        List<string> keys = new List<string>(ActivePowerUps.Keys);
        foreach (string key in keys)
        {
            ActiveEffects[key].RemoveEffect(_self);
            ActivePowerUps.Remove(key);
            ActiveEffects.Remove(key);
        }
    }
}
