using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBulletLogic : Projectile
{
    public PowerUp EffectToApply;
    
    protected override void OnHit(Collider2D col)
    {
        Teams otherTeam = (Team == Teams.Player) ? Teams.Enemy : Teams.Player;

        if (EffectToApply != null && col.tag == otherTeam.ToString())
        {
            var eem = col.GetComponent<EntityEffectManager>();
            if (eem != null)
            {
                eem.ApplyPowerUp(EffectToApply);
            }
        }
        Destroy(gameObject);
    }

    protected override void OnOutOfRange()
    {
        //Destroy the bullet
        Destroy(gameObject);
    }
}
