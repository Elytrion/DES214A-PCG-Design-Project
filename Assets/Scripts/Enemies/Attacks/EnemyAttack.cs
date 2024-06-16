using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : ScriptableObject
{
    public abstract void SetUpInstanceData(InstancedData inIData);

    public abstract void Attack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData);

    public abstract bool CanAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData);

    public abstract bool CanAttackOnDeaggro(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData);

    public abstract bool IsAttacking(InstancedData inIData);

    public virtual void EndAttack(InstancedData inIData)
    {
        // Do nothing by default
    }
}
