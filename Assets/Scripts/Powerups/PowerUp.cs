using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : ScriptableObject
{
    public string PowerupID;
    public string PickupText;
    public float Duration;
    public bool DurationStackable;
    public bool EffectStackable;
    public Sprite PowerupSprite;
    public bool isInstant = false;
    public bool overTime = false;
    public float overTimeTick;

    public virtual void ApplyEffect(Entity inEntity)
    {
        Debug.Log("Applying effect (" + PowerupID + ") to " + inEntity.gameObject.name);
    }

    public virtual void RemoveEffect(Entity inEntity)
    {
        Debug.Log("Removing effect (" + PowerupID + ") from " + inEntity.gameObject.name);
    }
}
