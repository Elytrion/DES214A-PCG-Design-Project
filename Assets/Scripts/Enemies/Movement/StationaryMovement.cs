using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyNoMovement", menuName = "EnemyLogic/Movement/NoMovement")]
public class StationaryMovement : EnemyMovement
{
    public override void Chase(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed, bool isColliding)
    {
        // Do nothing
    }

    public override bool CanReadjust(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed)
    {
        return false;
    }
}
