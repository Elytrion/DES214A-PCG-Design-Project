using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ImmunityPowerup", menuName = "Powerups/ImmunityPowerup")]
public class ImmunityPowerup : PowerUp
{
    public override void ApplyEffect(Entity inEntity)
    {
        inEntity.NoDamage = true;
        if (inEntity.tag == "Player")
        {
            var sr = inEntity.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.cyan;
        }
        else
        {
            var eb = inEntity.GetComponentInParent<EnemyBase>();
            if (eb != null)
                eb.TintSprites(Color.cyan);
        }
    }

    public override void RemoveEffect(Entity inEntity)
    {
        inEntity.NoDamage = false;
        if (inEntity.tag == "Player")
        {
            var sr = inEntity.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.white;
        }
        else
        {
            var eb = inEntity.GetComponentInParent<EnemyBase>();
            if (eb != null)
                eb.TintSprites(Color.white);
        }
    }
}
