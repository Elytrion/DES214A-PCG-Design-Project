using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpawnerAttack", menuName = "EnemyLogic/Attack/SpawnerAttack")]
public class SpawnerAttack : EnemyAttack
{
    public GameObject ItemToSpawnPrefab;
    public float SpawnCooldown = 1f;
    public float SpawnTotalCooldown = 2f;
    public float SpawnRange = 10.0f;
    public int SpawnsPerAttack = 1;

    public override void SetUpInstanceData(InstancedData inIData)
    {
        inIData.SetID<float>("cooldownTotalTimer", 0.0f);
        inIData.SetID<float>("cooldownTimer", 0.0f);
        inIData.SetID<int>("itemsSpawned", 0);
    }

    public override void Attack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        if (inIData.GetID<float>("cooldownTotalTimer") < SpawnTotalCooldown)
        {
            inIData.SetID<float>("cooldownTotalTimer", inIData.GetID<float>("cooldownTotalTimer") + Time.deltaTime);
            return;
        }
        
        if (inIData.GetID<int>("itemsSpawned") < SpawnsPerAttack)
        {
            inIData.SetID<float>("cooldownTimer", inIData.GetID<float>("cooldownTimer") + Time.deltaTime);
            if (inIData.GetID<float>("cooldownTimer") >= SpawnCooldown)
            {
                inIData.SetID<float>("cooldownTimer", 0.0f);
                Instantiate(ItemToSpawnPrefab, inPlayerTransform.position, Quaternion.identity);
                inIData.SetID<int>("itemsSpawned", inIData.GetID<int>("itemsSpawned") + 1);
            }
        }
        else
        {
            inIData.SetID<float>("cooldownTotalTimer", 0.0f);
            inIData.SetID<int>("itemsSpawned", 0);
        }
    }

    public override bool IsAttacking(InstancedData inIData)
    {
        return inIData.GetID<float>("cooldownTotalTimer") < SpawnTotalCooldown;
    }

    public override bool CanAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        return (Vector2.Distance(inEnemyTransform.position, inPlayerTransform.position) <= SpawnRange);
    }

    public override bool CanAttackOnDeaggro(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        return false;
    }
}
