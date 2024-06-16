using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ShootBulletsAttack", menuName = "EnemyLogic/Attack/ShootBulletsAttack")]
public class ShootBulletsAttack : EnemyAttack
{
    public GameObject BulletPrefab;

    public float BulletSpeed = 8f;
    public float BulletRange = 20f;
    public int BulletsPerShot = 3;
    public float BulletSpreadAngle = 0.25f;

    public float ShootCooldown = 1f;
    public float ShootRange = 100.0f;

    public override void SetUpInstanceData(InstancedData inIData)
    {
        inIData.SetID<float>("Timer", 0.0f);
    }

    public override void Attack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        inIData.SetID<float>("Timer", inIData.GetID<float>("Timer") + Time.deltaTime);
        if (inIData.GetID<float>("Timer") >=  ShootCooldown)
        {
            int bulletsLeft =  BulletsPerShot;
            float angleAdjust = 0.0f;
            //Odd number of bullets means fire the first one straight ahead
            if (bulletsLeft % 2 == 1)
            {
                FireBullet(0.0f, inEnemyTransform, inPlayerTransform);
                bulletsLeft--;
            }
            else //Even number of bullets means we need to adjust the angle slightly
            {
                angleAdjust = 0.5f;
            }
            //The rest of the bullets are spread out evenly
            while (bulletsLeft > 0)
            {
                FireBullet( BulletSpreadAngle * (bulletsLeft / 2) - ( BulletSpreadAngle * angleAdjust), inEnemyTransform, inPlayerTransform);
                FireBullet(- BulletSpreadAngle * (bulletsLeft / 2) + ( BulletSpreadAngle * angleAdjust), inEnemyTransform, inPlayerTransform);
                bulletsLeft -= 2; //Must do this afterwards, otherwise the angle will be wrong
            }

            inIData.SetID<float>("Timer", 0.0f);
        }
    }

    public override bool IsAttacking(InstancedData inIData)
    {
        return false;
    }

    public override bool CanAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
{
        return (Vector2.Distance(inEnemyTransform.position, inPlayerTransform.position) <=  ShootRange);
    }

    public override bool CanAttackOnDeaggro(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
{
        return false;
    }

    void FireBullet(float rotate, Transform inEnemyTransform, Transform inPlayerTransform)
    {
        var bullet = Instantiate( BulletPrefab, inEnemyTransform.position, Quaternion.identity);
        Vector2 dir = inPlayerTransform.position - inEnemyTransform.position;
        var fwd = RotateVector(dir.normalized, rotate);
        bullet.transform.up = fwd.normalized;
        bullet.GetComponent<Projectile>().LaunchProjectile(Teams.Enemy, BulletRange, BulletSpeed, fwd);          
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
