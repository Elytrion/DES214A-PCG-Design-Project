using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyRadiusMovement", menuName = "EnemyLogic/Movement/RadiusMovement")]
public class GoToRadiusMovement : EnemyMovement
{
    [SerializeField]
    private float _approachRadius = 2f;
    private Vector2 _targetMovePos;

    public override void Chase(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed, bool isColliding)
    {
        // set TargetPoint to closest point from the enemy to the player at the approach radius
        // Calculate the direction from the enemy to the player
        Vector2 direction = inPlayerTransform.position - inEnemyTransform.position;
        direction.Normalize();

        // Calculate the target position
        Vector2 pTransVec2 = new Vector2(inPlayerTransform.position.x, inPlayerTransform.position.y);

        _targetMovePos = pTransVec2 - direction * _approachRadius;

        // Move towards the target position
        if (Vector2.Distance(inEnemyTransform.position, _targetMovePos) > 0.1f)
        {
            if (Vector3.Distance(inEnemyTransform.position, inPlayerTransform.position) < _approachRadius)
                direction = -direction;

            inEnemyRB.velocity = direction * inBaseSpeed;
        }
        else if (Vector2.Distance(inEnemyTransform.position, _targetMovePos) < 0.1f)
        {
            inEnemyRB.velocity = Vector2.zero;
        }
    }

    public override bool CanReadjust(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed)
    {
        return (Vector3.Distance(inEnemyTransform.position, inPlayerTransform.position) > _approachRadius);
    }
}
