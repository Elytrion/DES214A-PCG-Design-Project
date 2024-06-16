using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New FreezeTimePowerup", menuName = "Powerups/FreezeTimePowerup")]
public class FreezeTimePowerup : PowerUp
{
    public override void ApplyEffect(Entity inEntity)
    {
        inEntity.NoDamage = true;
        EnemyDistributer ed = FindObjectOfType<EnemyDistributer>();
        foreach (EnemyBase eb in ed.SpawnedEnemiesScripts)
        {
            eb._entity.CanMove = false;
            eb._entity.CanAttack = false;
            eb.gameObject.GetComponent<InstancedData>().SetID<Vector2>("prevVelocity", eb._erb.velocity);
            eb._erb.velocity = Vector2.zero;
        }

        var allBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in allBullets)
        {
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public override void RemoveEffect(Entity inEntity)
    {
        inEntity.NoDamage = false;
        EnemyDistributer ed = FindObjectOfType<EnemyDistributer>();
        foreach (EnemyBase eb in ed.SpawnedEnemiesScripts)
        {
            eb._entity.CanMove = true;
            eb._entity.CanAttack = true;
            eb._erb.velocity = eb.gameObject.GetComponent<InstancedData>().GetID<Vector2>("prevVelocity");
        }

        var allBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in allBullets)
        {
            var projectile = bullet.GetComponent<Projectile>();
            if (projectile != null)
                bullet.GetComponent<Rigidbody2D>().velocity = projectile.IntitalProjectileSpeed;
        }
    }
}
