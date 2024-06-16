using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerShootingAttack", menuName = "Player/PlayerAttack/ShootingAttack")]
public class ShootingAttack : PlayerAttack
{
    public int BulletsFiredPerShot = 1;
    public float BulletSpreadAngle = 0.25f;
    public float BulletSpeed = 8f;
    public float BulletRange = 20f;

    public override void SetUpInstanceData(InstancedData inIData)
    {
        
    }

    public override void UseAttack(Player inPlayer, Transform inPlayerTransform, Transform inPlayerArmTransform, GameObject inAttackProjectilePrefab, int PlayerDamage, InstancedData inIData)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int bulletsLeft = BulletsFiredPerShot;
        float angleAdjust = 0.0f;
        //Odd number of bullets means fire the first one straight ahead
        if (bulletsLeft % 2 == 1)
        {
            FireBullet(0.0f, inPlayerTransform, pos, inAttackProjectilePrefab, PlayerDamage);
            bulletsLeft--;
        }
        else //Even number of bullets means we need to adjust the angle slightly
        {
            angleAdjust = 0.5f;
        }
        //The rest of the bullets are spread out evenly
        while (bulletsLeft > 0)
        {
            FireBullet(BulletSpreadAngle * (bulletsLeft / 2) - (BulletSpreadAngle * angleAdjust), inPlayerTransform, pos, inAttackProjectilePrefab, PlayerDamage);
            FireBullet(-BulletSpreadAngle * (bulletsLeft / 2) + (BulletSpreadAngle * angleAdjust), inPlayerTransform, pos, inAttackProjectilePrefab, PlayerDamage);
            bulletsLeft -= 2; //Must do this afterwards, otherwise the angle will be wrong
        }
    }

    public override bool CanUseAttack(Player inPlayer, Transform inPlayerTransform, Transform inPlayerArmTransform, InstancedData inIData)
    {
        return true;
    }


    void FireBullet(float rotate, Transform inPlayerTransform, Vector3 inTarget, GameObject inAttackProjectilePrefab, int PlayerDamage)
    {
        if (inAttackProjectilePrefab == null)
            return;
        
        var bullet = Instantiate(inAttackProjectilePrefab, inPlayerTransform.position, Quaternion.identity);
        Vector2 dir = inTarget - inPlayerTransform.position;
        var fwd = RotateVector(dir.normalized, rotate);
        bullet.transform.up = fwd.normalized;
        bullet.GetComponent<Projectile>().LaunchProjectile(Teams.Player, BulletRange, BulletSpeed, fwd, PlayerDamage);
    }

    Vector2 RotateVector(Vector2 vec, float Angle)
    {
        //x2 = cos(A) * x1 - sin(A) * y1
        var newX = Mathf.Cos(Angle) * vec.x - Mathf.Sin(Angle) * vec.y;

        //y2 = sin(A) * x1 + cos(B) * y1;
        var newY = Mathf.Sin(Angle) * vec.x + Mathf.Cos(Angle) * vec.y;

        return new Vector2(newX, newY);
    }
}
