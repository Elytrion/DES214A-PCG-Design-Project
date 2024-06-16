using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyCollision : MonoBehaviour
{
    [SerializeField]
    private EnemyBase _eb;

    private void OnCollisionStay2D(Collision2D col)
    {
        if (_eb != null)
            _eb.OnEnemyCollisionStay(col);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (_eb != null)
            _eb.OnEnemyCollisionExit(col);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_eb != null)
            _eb.OnEnemyCollisionEnter(col);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_eb != null)
            _eb.OnEnemyTriggerEnter(col);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (_eb != null)
            _eb.OnEnemyTriggerStay(col);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (_eb != null)
            _eb.OnEnemyTriggerExit(col);
    }
}


