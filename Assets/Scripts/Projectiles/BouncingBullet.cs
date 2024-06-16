using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BouncingBullet : Projectile
{
    protected override void OnHit(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            Vector2 velocity = RB.velocity;
            Vector2 normal = velocity.normalized;
            float speed = velocity.magnitude;

            bool deflectLeft = Random.value < 0.5f;

            float deflectionAngle = (deflectLeft ? -60f : 60f) * Mathf.Deg2Rad;

            Vector2 deflection = new Vector2(Mathf.Cos(deflectionAngle), Mathf.Sin(deflectionAngle));

            Vector2 deflectedVelocity = deflection * speed;

            RB.velocity = deflectedVelocity;

            transform.up = deflection;
            
            return;
        }
        if (col.tag == "Prop" || col.tag == "Wall")
        {
            Vector2 velocity = RB.velocity;
            Vector2 colNorm = col.transform.position - transform.position;
            
            if (col.tag == "Wall")
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity.normalized, velocity.magnitude);
                colNorm = hit.normal;
            }
            colNorm.Normalize();
            float speed = velocity.magnitude;
            Vector2 reflectedDirection = Vector2.Reflect(velocity.normalized, colNorm);
            Vector2 reflectedVelocity = reflectedDirection * speed;
            RB.velocity = reflectedVelocity;
            transform.up = reflectedDirection;
            return;
        }

        Destroy(gameObject);     
    }

    protected override void OnOutOfRange()
    {
        Destroy(gameObject);   
    }
}
