using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningBullet : Projectile
{
    public List<EnemyAttack> DoNotSpawnTypes;

    protected override void OnHit(Collider2D col)
    {
        BulletDamage = 0;
        var EnemySpawner = FindObjectOfType<EnemyDistributer>();
        if (EnemySpawner != null)
        {
            EnemySpawner.SpawnEnemy(transform.position, transform.position);
        }
        Destroy(gameObject);
    }

    protected override void OnOutOfRange()
    {
        //Destroy the bullet
        BulletDamage = 0;
        var EnemySpawner = FindObjectOfType<EnemyDistributer>();
        if (EnemySpawner != null)
        {
            EnemySpawner.SpawnEnemy(transform.position, transform.position);
        }
        Destroy(gameObject);
    }
}
