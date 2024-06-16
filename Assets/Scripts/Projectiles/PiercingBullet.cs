using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingBullet : Projectile
{
    protected override void OnHit(Collider2D col)
    {
        // dont do anything
    }

    protected override void OnOutOfRange()
    {
        Destroy(gameObject);
    }
}
