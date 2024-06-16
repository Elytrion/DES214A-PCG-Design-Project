using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpeedPowerup", menuName = "Powerups/SpeedPowerup")]
public class SpeedPowerup : PowerUp
{
    public int SpeedAmount;
    public bool Freeze = false;
    public bool Reverse = false;
    public bool Permanent = false;

    public override void ApplyEffect(Entity inEntity)
    {
        GameObject obj = inEntity.gameObject;

        if (Freeze)
        {
            if (obj.tag == "Player")
            {
                var player = obj.GetComponent<Player>();
                var idata = obj.GetComponent<InstancedData>();
                if (idata != null)
                {
                    idata.SetID<float>("speed", player.Speed);
                    player.Speed = 0;
                }
                var player_sr = obj.GetComponent<SpriteRenderer>();
                if (player_sr != null)
                {
                    player_sr.color = Color.magenta;
                }
            }
            else if (obj.tag == "Enemy")
            {
                var enemy = obj.GetComponent<EnemyBase>();
                var idata = obj.GetComponent<InstancedData>();
                if (idata != null)
                {
                    idata.SetID<float>("speed", enemy.MovementSpeed);
                    enemy.MovementSpeed = 0;
                    enemy._erb.velocity = Vector2.zero;
                }
            }
            return;
        }

        if (Reverse)
        {
            if (obj.tag == "Player")
            {
                obj.GetComponent<Player>().Speed = -obj.GetComponent<Player>().Speed;
            }
            else if (obj.tag == "Enemy")
            {
                obj.GetComponent<EnemyBase>().MovementSpeed = -obj.GetComponent<EnemyBase>().MovementSpeed;
            }
            return;
        }
        
        if (obj.tag == "Player")
        {
            var idata = obj.GetComponent<InstancedData>();
            if (idata != null)
            {
                idata.SetID<float>("speed_base", obj.GetComponent<Player>().Speed);
            }
            obj.GetComponent<Player>().Speed += SpeedAmount;

        }
        else if (obj.tag == "Enemy")
        {
            obj.GetComponentInParent<EnemyBase>().MovementSpeed += SpeedAmount;
        }
    }

    public override void RemoveEffect(Entity inEntity)
    {
        if (Permanent)
            return;

        GameObject obj = inEntity.gameObject;

        if (Freeze)
        {
            if (obj.tag == "Player")
            {
                var player = obj.GetComponent<Player>();
                var idata = obj.GetComponent<InstancedData>();
                if (idata != null)
                {        
                    player.Speed = idata.GetID<float>("speed");
                }
                var player_sr = obj.GetComponent<SpriteRenderer>();
                if (player_sr != null)
                {
                    player_sr.color = Color.white;
                }
            }
            else if (obj.tag == "Enemy")
            {
                var enemy = obj.GetComponent<EnemyBase>();
                var idata = obj.GetComponent<InstancedData>();
                if (idata != null)
                {
                    enemy.MovementSpeed = idata.GetID<float>("speed");
                }
            }
            return;
        }

        if (Reverse)
        {
            if (obj.tag == "Player")
            {
                obj.GetComponent<Player>().Speed = -obj.GetComponent<Player>().Speed;
            }
            else if (obj.tag == "Enemy")
            {
                obj.GetComponent<EnemyBase>().MovementSpeed = -obj.GetComponent<EnemyBase>().MovementSpeed;
            }
            return;
        }

        if (obj.tag == "Player")
        {
            var idata = obj.GetComponent<InstancedData>();
            if (idata != null)
            {
                obj.GetComponent<Player>().Speed = idata.GetID<float>("speed_base");
            }
        }
        else if (obj.tag == "Enemy")
        {
            obj.GetComponentInParent<EnemyBase>().MovementSpeed -= SpeedAmount;
        }
    }
}
