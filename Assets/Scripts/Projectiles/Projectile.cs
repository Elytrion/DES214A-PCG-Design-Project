using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Teams { Player, Enemy, None }

public abstract class Projectile : MonoBehaviour
{
    public Teams Team = Teams.Player;
    [HideInInspector]
    public float BulletRangeLeft;
    public int BulletDamage = 1;

    protected Rigidbody2D RB;
    public Vector2 IntitalProjectileSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    public void LaunchProjectile(Teams inTeam, float inRange, float inSpeed, Vector2 inDirection, int inDamage = -1)
    {
        RB = GetComponent<Rigidbody2D>();
        Team = inTeam;
        BulletRangeLeft = inRange;
        
        if (inDamage > 0)
            BulletDamage = inDamage;
        
        IntitalProjectileSpeed = inDirection * inSpeed;
        RB.velocity = IntitalProjectileSpeed;
    }

    private void Update()
    {
        //Destroy the bullet after it has travelled far enough
        BulletRangeLeft -= (Time.deltaTime * RB.velocity.magnitude);
        if (BulletRangeLeft < 0)
            OnOutOfRange();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //No friendly fire
        if (col.isTrigger || col.tag == Team.ToString())
            return;

        if (col.tag == "Player" && Team != Teams.Player)
        {
            col.GetComponent<Entity>().ReduceHealth(BulletDamage);
        }
        else if (col.tag == "Enemy" && Team != Teams.Enemy)
        {
            col.GetComponent<Entity>().ReduceHealth(BulletDamage);
        }
        else if (col.tag == "Prop")
        {
            if (col.GetComponent<Entity>() != null)
            {
                col.GetComponent<Entity>().ReduceHealth(BulletDamage);
            }
        }

        OnHit(col);
    }

    protected abstract void OnHit(Collider2D col);

    protected abstract void OnOutOfRange();

    
}
