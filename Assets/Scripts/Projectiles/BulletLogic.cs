using UnityEngine;

public class BulletLogic : Projectile
{

    protected override void OnHit(Collider2D col)
    {
        Destroy(gameObject);
    }

    protected override void OnOutOfRange()
    {
        Destroy(gameObject);
    }
}
