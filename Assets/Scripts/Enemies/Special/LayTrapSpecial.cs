using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LayTrapSpecial", menuName = "EnemyLogic/Special/LayTrapSpecial")]
public class LayTrapSpecial : EnemySpecial
{
    public GameObject TrapPrefab;
    public float CoolDownTimer;

    public override void SetUpInstanceData(InstancedData inIData)
    {
        inIData.SetID<float>("cooldown", 0.0f);
        inIData.SetID<Vector3>("prevPos", Vector3.zero);
    }

    public override void OnAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        
    }

    public override void OnMove(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        inIData.SetID<Vector3>("prevPos", inEnemyTransform.position);
        Instantiate(TrapPrefab, inEnemyTransform.position, Quaternion.identity);
        inIData.SetID<float>("cooldown", CoolDownTimer);
    }

    public override void OnDeath(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {

    }

    public override bool CanUseSpecial(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        float cooldown = inIData.GetID<float>("cooldown");
        float distanceFromPrevPos = Vector3.Distance(inIData.GetID<Vector3>("prevPos"), inEnemyTransform.position);
        if (cooldown <= 0.0f && distanceFromPrevPos > 1.0f)
        {
            return true;
        }
        else
        {
            inIData.SetID<float>("cooldown", inIData.GetID<float>("cooldown") - Time.deltaTime);
            return false;
        }
    }
}
