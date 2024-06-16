using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ReplaceBulletPowerup", menuName = "Powerups/ReplaceBulletPowerup")]
public class ReplaceBulletPowerup : PowerUp
{
    public GameObject BulletPrefab;

    public override void ApplyEffect(Entity inEntity)
    {
        var player = inEntity.GetComponent<Player>();
        if (player == null)
            return;

        InstancedData playerBB = player.GetComponent<InstancedData>();
        playerBB.SetID<GameObject>("bulletPrefab", player._bulletPrefab);
        player._bulletPrefab = BulletPrefab;
    }

    public override void RemoveEffect(Entity inEntity)
    {
        var player = inEntity.GetComponent<Player>();
        if (player == null)
            return;

        InstancedData playerBB = player.GetComponent<InstancedData>();
        player._bulletPrefab = playerBB.GetID<GameObject>("bulletPrefab");
    }
}
