using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthPU", menuName = "Powerups/HealthPU")]
public class HealthPowerup : PowerUp
{
    public int HealthAmount;

    public override void ApplyEffect(Entity inEntity)
    {
        inEntity.Health += HealthAmount;
        inEntity.Health = Mathf.Clamp(inEntity.Health, 0, inEntity.MaxHealth);
    }
}
