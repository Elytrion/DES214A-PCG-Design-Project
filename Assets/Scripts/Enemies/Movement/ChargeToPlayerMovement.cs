using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyChargeMovement", menuName = "EnemyLogic/Movement/ChargeMovement")]
public class ChargeToPlayerMovement : EnemyMovement
{
    [SerializeField]
    private float _chargeSpeed = 8f;
    [SerializeField]
    private float _chargeDelay = 1.0f;
    [SerializeField]
    private float _chargeRecoveryTime = 2.0f;

    private float _timer = 0.0f;
    private float _recoveryTimer = 0.0f;
    private Vector2 _targetMovePos;
    private bool isCharging = false;

    public override void Chase(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed, bool isColliding)
    {
        // set the targetmovepos if not already set
        // wait for delay
        // once delay is over, charge directly to the point, if colliding, stop immediately
        // if at point, wait for recovery time, then repeat

        // If the enemy is not charging, set the target position and start charging after the delay
        if (!isCharging)
        {
            if (_recoveryTimer >= 0.0f)
            {
                _recoveryTimer -= Time.deltaTime;
                return;
            }         
            // Set target position if not already set
            if (_targetMovePos == Vector2.zero)
            {
                _targetMovePos = inPlayerTransform.position;
            }
            // Increment the timer
            _timer += Time.deltaTime;
            // If the timer is greater than the delay, start charging
            if (_timer >= _chargeDelay)
            {
                isCharging = true;
                _timer = 0f;
            }
        }
        else
        {
            // If colliding with something or has reached the target position, start recovery
            if (isColliding || Vector2.Distance(inEnemyTransform.position, _targetMovePos) < 0.1f)
            {
                isCharging = false;
                _timer = 0f;
                _recoveryTimer = _chargeRecoveryTime;
                _targetMovePos = Vector2.zero;
                inEnemyRB.velocity = Vector2.zero;
            }
            // Otherwise, continue charging towards the target
            else
            {
                Vector2 direction = (_targetMovePos - (Vector2)inEnemyTransform.position).normalized;
                inEnemyRB.velocity = direction * _chargeSpeed;
            }
        }
    }

    public override bool CanReadjust(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, float inBaseSpeed)
    {
        return true;
    }
}
