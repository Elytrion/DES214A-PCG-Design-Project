using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement : ScriptableObject
{
    public abstract void Chase(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed, bool isColliding);

    public abstract bool CanReadjust(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed);
}
