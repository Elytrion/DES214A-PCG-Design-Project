using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New IncreaseDamagePowerup", menuName = "Powerups/IncreaseDamagePowerup")]
public class IncreaseDamagePowerup : PowerUp
{
    public int DmgIncrease;

    public override void ApplyEffect(Entity inEntity)
    {
        var player = inEntity.GetComponent<Player>();
        if (player == null)
            return;
        player.playerDamage += DmgIncrease;
    }

    public override void RemoveEffect(Entity inEntity)
    {
        var player = inEntity.GetComponent<Player>();
        if (player == null)
            return;
        player.playerDamage -= DmgIncrease;
    }
}
