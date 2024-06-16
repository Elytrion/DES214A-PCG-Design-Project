using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MaxHealthPU", menuName = "Powerups/MaxHealthPU")]
public class MaxHealthPowerup : PowerUp
{
    public int MaxHealthAmount;

    public override void ApplyEffect(Entity inEntity)
    {
        inEntity.MaxHealth += MaxHealthAmount;
        inEntity.Health += MaxHealthAmount;
        inEntity.Health = Mathf.Clamp(inEntity.Health, 0, inEntity.MaxHealth);
    }
}
