using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemySpecial : ScriptableObject
{
    public abstract void SetUpInstanceData(InstancedData inIData);

    public abstract void OnAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData);

    public abstract void OnMove(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData);

    public abstract void OnDeath(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData);

    public abstract bool CanUseSpecial(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData);
}
