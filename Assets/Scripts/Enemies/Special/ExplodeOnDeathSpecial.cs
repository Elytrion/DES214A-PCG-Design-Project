using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExplodeOnDeathSpecial", menuName = "EnemyLogic/Special/ExplodeOnDeathSpecial")]
public class ExplodeOnDeathSpecial : EnemySpecial
{
    public GameObject ExplosiveBulletPrefab;
    public int NumberOfBullets;
    public float ExplosiveSpeed;
    public int ExplosiveDamage;
    public float ExplosiveRange;
    public float Angle;

    public override void SetUpInstanceData(InstancedData inIData)
    {
        
    }
    public override void OnAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        
    }
    public override void OnMove(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        
    }

    public override void OnDeath(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        int bulletsLeft = NumberOfBullets;
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
            FireBullet(Angle * (bulletsLeft / 2) - (Angle * angleAdjust), inEnemyTransform, inPlayerTransform);
            FireBullet(-Angle * (bulletsLeft / 2) + (Angle * angleAdjust), inEnemyTransform, inPlayerTransform);
            bulletsLeft -= 2; //Must do this afterwards, otherwise the angle will be wrong
        }
    }

    public override bool CanUseSpecial(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        return true;
    }

    void FireBullet(float rotate, Transform inEnemyTransform, Transform inPlayerTransform)
    {
        var bullet = Instantiate(ExplosiveBulletPrefab, inEnemyTransform.position, Quaternion.identity);
        Vector2 dir = inPlayerTransform.position - inEnemyTransform.position;
        var fwd = RotateVector(dir.normalized, rotate);
        bullet.transform.up = fwd.normalized;
        bullet.GetComponent<Rigidbody2D>().velocity = fwd * ExplosiveSpeed;
        bullet.GetComponent<Projectile>().BulletRangeLeft = ExplosiveRange;
        bullet.GetComponent<Projectile>().Team = Teams.None;
        bullet.GetComponent<Projectile>().BulletDamage = ExplosiveDamage;
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
