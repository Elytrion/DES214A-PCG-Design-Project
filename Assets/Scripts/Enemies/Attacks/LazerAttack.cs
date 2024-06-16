using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LazerAttack", menuName = "EnemyLogic/Attack/LazerAttack")]
public class LazerAttack : EnemyAttack
{
    public GameObject _lazerPrefab;

    public float _lazerRange = 20f;

    public float _chargeTime;
    public float _lockOnTime;
    public float _fadeTime;
    public float _coolDownTime;

    public float _flashSpeed;
    public float _lazerStartingWidth;
    public float _lazerEndingWidth;
    public Color _lazerStartingColor;
    public Color _lazerEndingColor;

    public enum LazerState
    {
        Idle = 0,
        Charging,
        LockedOn,
        Firing,
        Fading,
        None
    }

    public override void SetUpInstanceData(InstancedData inIData)
    {
        inIData.SetID<LazerState>("_currentState", LazerState.Idle);
        inIData.SetID<float>("_coolDownTimer", -0.5f);
        inIData.SetID<float>("_chargeTimer", -0.5f);
        inIData.SetID<float>("_fadeTimer", -0.5f);
        inIData.SetID<float>("_lockOnTimer", -0.5f);
        inIData.SetID<GameObject>("_lazerInstance", null);
        inIData.SetID<LazerLogic>("_lazerLine", null);
        inIData.SetID<Vector3>("_lazerTargetPos", Vector3.zero);
    }

    public override void Attack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        switch (inIData.GetID<LazerState>("_currentState"))
        {
            case LazerState.Idle:
                StartAttack(inEnemyTransform, inEnemyRB, inPlayerTransform, inEnemyArm, inIData);               
                break;
            case LazerState.Charging:
                ManageAim(inEnemyTransform, inEnemyRB, inPlayerTransform, inEnemyArm, inIData);
                break;
            case LazerState.LockedOn:
                LockedOnStage(inEnemyTransform, inEnemyRB, inPlayerTransform, inEnemyArm, inIData);
                break;
            case LazerState.Firing:
                FireLazerStage(inEnemyTransform, inEnemyRB, inPlayerTransform, inEnemyArm, inIData);
                break;
            case LazerState.Fading:
                FadeLazerStage(inEnemyTransform, inEnemyRB, inPlayerTransform, inEnemyArm, inIData);
                break;
            default:
                break;
        }
    }

    public override bool IsAttacking(InstancedData inIData)
    {
        return inIData.GetID<LazerState>("_currentState") != LazerState.Idle;
    }

    public override bool CanAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        if (inIData.GetID<float>("_coolDownTimer") >= 0 && inIData.GetID<LazerState>("_currentState") == LazerState.Idle)
        {
            inIData.SetID<float>("_coolDownTimer", inIData.GetID<float>("_coolDownTimer") - Time.deltaTime);
            return false;
        } 
        return true;
    }

    public override bool CanAttackOnDeaggro(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        return inIData.GetID<LazerState>("_currentState") != LazerState.Idle;
    }

    private void StartAttack(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        // Initialize here
        inIData.SetID<GameObject>("_lazerInstance", Instantiate(_lazerPrefab, inEnemyTransform));
        inIData.SetID<LazerLogic>("_lazerLine", inIData.GetID<GameObject>("_lazerInstance").GetComponent<LazerLogic>());
        if (inIData.GetID<LazerLogic>("_lazerLine") != null)
        {
            inIData.GetID<LazerLogic>("_lazerLine").SetWidth( _lazerStartingWidth);
            inIData.GetID<LazerLogic>("_lazerLine").SetColor( _lazerStartingColor);
        }
        inIData.SetID<LazerState>("_currentState", LazerState.Charging);
    }

    private void ManageAim(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        inIData.SetID<float>("_chargeTimer", inIData.GetID<float>("_chargeTimer") + Time.deltaTime);
        
        Vector2 direction = (inPlayerTransform.position - inEnemyTransform.position).normalized;
        Vector3 aimPos = inEnemyTransform.position + (Vector3)direction * _lazerRange;

        int layerMask = ~inIData.GetID<LazerLogic>("_lazerLine").IgnoredLayers; // replace with your layer names
        RaycastHit2D hit = Physics2D.CircleCast(inEnemyTransform.position,  _lazerStartingWidth, direction,  _lazerRange, layerMask);
        if (hit)
        {
            aimPos = hit.point;
        }

        if (inIData.GetID<LazerLogic>("_lazerLine") != null)
        {
            inIData.GetID<LazerLogic>("_lazerLine").SetWidth( _lazerStartingWidth);
            inIData.GetID<LazerLogic>("_lazerLine").SetColor( _lazerStartingColor);
            inIData.GetID<LazerLogic>("_lazerLine").SetTarget(aimPos);
        }

        if (inIData.GetID<float>("_chargeTimer") >= _chargeTime)
            inIData.SetID<LazerState>("_currentState", LazerState.LockedOn);
    }

    private void LockedOnStage(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        inIData.SetID<float>("_lockOnTimer", inIData.GetID<float>("_lockOnTimer") + Time.deltaTime);

        if (inIData.GetID<LazerLogic>("_lazerLine") != null)
        {
            float intensity = 0.5f * (1f + Mathf.Sin(Time.time *  _flashSpeed));
            Color baseColor =  _lazerStartingColor;
            Color flashColor = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * intensity);
            inIData.GetID<LazerLogic>("_lazerLine").SetColor(flashColor);
        }

        if (inIData.GetID<float>("_lockOnTimer") >=  _lockOnTime)
            inIData.SetID<LazerState>("_currentState", LazerState.Firing);
    }

    private void FireLazerStage(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        if (inIData.GetID<LazerLogic>("_lazerLine") != null)
        {
            inIData.GetID<LazerLogic>("_lazerLine").SetColor( _lazerEndingColor);
            inIData.GetID<LazerLogic>("_lazerLine").SetWidth( _lazerEndingWidth);
        }

        Vector2 direction = (inIData.GetID<LazerLogic>("_lazerLine").aimPos - inEnemyTransform.position).normalized;
        int layerMask = ~inIData.GetID<LazerLogic>("_lazerLine").IgnoredLayers;
        RaycastHit2D hit = Physics2D.CircleCast(inEnemyTransform.position,  _lazerEndingWidth, direction,  _lazerRange, layerMask);

        if (hit)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<Entity>().ReduceHealth(inIData.GetID<LazerLogic>("_lazerLine").LazerDamage);
                inIData.GetID<LazerLogic>("_lazerLine").SpawnObject();
            }
        }
        inIData.SetID<LazerState>("_currentState", LazerState.Fading);
    }

    private void FadeLazerStage(Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        inIData.SetID<float>("_fadeTimer", inIData.GetID<float>("_fadeTimer") + Time.deltaTime);
        float alpha = ( _fadeTime - inIData.GetID<float>("_fadeTimer")) /  _fadeTime;
        // create a new color using the old color RGB values but with the new alpha
        Color fadedColor = new Color( _lazerEndingColor.r,  _lazerEndingColor.g,  _lazerEndingColor.b, alpha);
        inIData.GetID<LazerLogic>("_lazerLine").SetColor(fadedColor);

        if (inIData.GetID<float>("_fadeTimer") >=  _fadeTime)
        {
            ResetAttack(inIData);
            inIData.SetID<float>("_coolDownTimer", _coolDownTime);
        }
    }

    private void ResetAttack(InstancedData inIData)
    {
        inIData.SetID<LazerState>("_currentState", LazerState.Idle);
        inIData.SetID<float>("_chargeTimer", -0.5f);
        inIData.SetID<float>("_fadeTimer", -0.5f);
        inIData.SetID<float>("_lockOnTimer", -0.5f);
        if (inIData.GetID<GameObject>("_lazerInstance") != null)
        {
            Destroy(inIData.GetID<GameObject>("_lazerInstance"));
        }
    }

}
