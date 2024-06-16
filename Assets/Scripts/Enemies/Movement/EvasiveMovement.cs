using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyEvasiveMovement", menuName = "EnemyLogic/Movement/EvasiveMovement")]
public class EvasiveMovement : EnemyMovement
{
    [SerializeField]
    private float _zigzagSpeed = 5f;
    [SerializeField]
    private float _zigzagMagnitude = 0.5f;
    [SerializeField]
    private float _approachRadius = 2f;
    [SerializeField]
    private float _delayAmt = 2f;

    private Vector2 _targetPosition;
    private float _timer;
    private float _delayTimer;

    public override void Chase(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed, bool isColliding)
    {
        if (_delayTimer > 0f)
        {
            _delayTimer -= Time.deltaTime;
            return;
        }

        Vector2 direction = (inPlayerTransform.position - inEnemyTransform.position).normalized;

        Vector2 pTransVec2 = new Vector2(inPlayerTransform.position.x, inPlayerTransform.position.y);
        _targetPosition = pTransVec2 - direction * _approachRadius;

        // Increment timer for the sine function
        _timer += Time.deltaTime;
        if (Vector2.Distance(inEnemyTransform.position, _targetPosition) > 0.1f)
        {
            Vector2 zigzag = new Vector2(-direction.y, direction.x) * Mathf.Sin(_timer * _zigzagSpeed) * _zigzagMagnitude;
            Vector2 movement = (direction + zigzag) * inBaseSpeed;
            inEnemyRB.velocity = movement;
        }
        else if (Vector2.Distance(inEnemyTransform.position, _targetPosition) < 0.1f)
        {
            inEnemyRB.velocity = Vector2.zero;
            _delayTimer = _delayAmt;
        }
    }

    public override bool CanReadjust(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed)
    {
        return true;
    }
}
