using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBulletLogic : Projectile
{
    public GameObject AOEToSpawn;
    public float Fuse;

    protected override void OnHit(Collider2D col)
    {
        OnOutOfRange();
    }

    protected override void OnOutOfRange()
    {
        RB.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Fuse -= Time.deltaTime;
        if (Fuse <= 0)
        {
            Instantiate(AOEToSpawn, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
