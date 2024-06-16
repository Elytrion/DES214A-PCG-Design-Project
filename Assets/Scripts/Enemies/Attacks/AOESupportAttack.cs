using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AOESupportAttack", menuName = "EnemyLogic/Attack/AOESupportAttack")]
public class AOESupportAttack : EnemyAttack
{
    public bool HitsEnemies = true;
    public GameObject ConnectorLine;
    public PowerUp EffectToApply;
    public float Radius;
    public Color ConnectorLineColor = Color.red;
    public float ConnectorLineWidth = 0.1f;
    
    public bool HitsPlayer = false;
    public PowerUp PlayerEffectToApply;
    public Color PlayerConnectorLineColor = Color.red;
    public float PlayerConnectorLineWidth = 0.1f;

    public override void SetUpInstanceData(InstancedData inIData)
    {
        inIData.SetID<List<Entity>>("AllNearbyEnemies", new List<Entity>());
        inIData.SetID<List<GameObject>>("AllConnectorLines", new List<GameObject>());
        inIData.SetID<List<LazerLogic>>("AllConnectorRenderers", new List<LazerLogic>());
        
        var line = Instantiate(ConnectorLine, Vector3.zero, Quaternion.identity);
        inIData.SetID<GameObject>("PlayerConnector", line);
        inIData.SetID<LazerLogic>("PlayerConnectorRenderer", line.GetComponent<LazerLogic>());
        GameObject playerConnector = inIData.GetID<GameObject>("PlayerConnector");
        playerConnector.SetActive(false);
    }

    public override void Attack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, Transform inEnemyArm, InstancedData inIData)
    {
        if (HitsEnemies)
        {
            List<Entity> prevNearbyEnemies = inIData.GetID<List<Entity>>("AllNearbyEnemies");
            List<Entity> allNearbyEnemies = FindAllNearbyEnemies(inEnemyBase, inEnemyTransform, inIData);

            foreach (Entity enemy in prevNearbyEnemies)
            {
                if (!allNearbyEnemies.Contains(enemy))
                {
                    var eem = enemy.GetComponent<EntityEffectManager>();
                    if (eem != null)
                        eem.RemovePowerup(EffectToApply);
                }
            }

            inIData.SetID<List<Entity>>("AllNearbyEnemies", allNearbyEnemies);


            List<GameObject> allConnectorLines = inIData.GetID<List<GameObject>>("AllConnectorLines");
            List<LazerLogic> allConnectorRenderers = inIData.GetID<List<LazerLogic>>("AllConnectorRenderers");

            // Remove any lines that are no longer needed
            if (allConnectorLines.Count > allNearbyEnemies.Count)
            {
                for (int i = allNearbyEnemies.Count; i < allConnectorLines.Count; i++)
                {
                    Destroy(allConnectorLines[i]);
                    allConnectorLines.RemoveAt(i);
                    allConnectorRenderers.RemoveAt(i);
                }
            }

            // Add any lines that are needed
            if (allConnectorLines.Count < allNearbyEnemies.Count)
            {
                for (int i = allConnectorLines.Count; i < allNearbyEnemies.Count; i++)
                {
                    GameObject newConnectorLine = Instantiate(ConnectorLine, inEnemyTransform.position, Quaternion.identity);
                    allConnectorLines.Add(newConnectorLine);
                    allConnectorRenderers.Add(newConnectorLine.GetComponent<LazerLogic>());
                }
            }

            // Update all lines
            for (int i = 0; i < allNearbyEnemies.Count; i++)
            {
                allConnectorLines[i].transform.position = inEnemyTransform.position;
                allConnectorRenderers[i].SetTarget(allNearbyEnemies[i].transform.position);
                allConnectorRenderers[i].SetColor(ConnectorLineColor);
                allConnectorRenderers[i].SetWidth(ConnectorLineWidth);
                EntityEffectManager eem = allNearbyEnemies[i].GetComponent<EntityEffectManager>();
                if (eem != null)
                {
                    if (!eem.HasPowerup(EffectToApply) && EffectToApply != null)
                    {
                        eem.ApplyPowerUp(EffectToApply);
                    }
                }
            }
        }

        if (HitsPlayer)
        {
            // check if there is a player nearby
            if (Vector3.Distance(inPlayerTransform.position, inEnemyTransform.position) <= Radius)
            {
                GameObject playerConnector = inIData.GetID<GameObject>("PlayerConnector");
                LazerLogic playerConnectorRenderer = inIData.GetID<LazerLogic>("PlayerConnectorRenderer");
                playerConnector.SetActive(true);
                playerConnector.transform.position = inEnemyTransform.position;
                playerConnectorRenderer.SetTarget(inPlayerTransform.position);
                playerConnectorRenderer.SetColor(PlayerConnectorLineColor);
                playerConnectorRenderer.SetWidth(PlayerConnectorLineWidth);
                EntityEffectManager eem = inPlayerTransform.GetComponent<EntityEffectManager>();
                if (eem != null && PlayerEffectToApply != null)
                {
                    if (!eem.HasPowerup(PlayerEffectToApply))
                    {
                        eem.ApplyPowerUp(PlayerEffectToApply);
                    }
                }
            }
            else
            {
                GameObject playerConnector = inIData.GetID<GameObject>("PlayerConnector");
                playerConnector.SetActive(false);
                EntityEffectManager eem = inPlayerTransform.GetComponent<EntityEffectManager>();
                if (eem != null && PlayerEffectToApply != null)
                {
                    if (eem.HasPowerup(PlayerEffectToApply))
                    {
                        eem.RemovePowerup(PlayerEffectToApply);
                    }
                }
            }
            
        }
    }

    public override bool IsAttacking(InstancedData inIData)
    {
        return true;
    }

    public override bool CanAttack(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        return true;
    }

    public override bool CanAttackOnDeaggro(EnemyBase inEnemyBase, Transform inEnemyTransform, Rigidbody2D inEnemyRB, Transform inPlayerTransform, InstancedData inIData)
    {
        return false;
    }

    private List<Entity> FindAllNearbyEnemies(EnemyBase inEnemyBase, Transform inEnemyTransform, InstancedData inIData)
    {
        inIData.SetID<Entity>("NearbyPlayer", null);
        List<Entity> allNearbyEnemies = new List<Entity>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(inEnemyTransform.position, Radius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.tag != "Player" && collider.tag != "Enemy")
            {
                continue;
            }

            Entity enemy = collider.GetComponentInParent<Entity>();
            if (enemy != null && enemy != inEnemyBase._entity)
            {
                if (enemy.tag == "Enemy")
                {
                    var eb = enemy.GetComponentInParent<EnemyBase>();
                    if (eb != null && eb.AttackLogic != null && eb.AttackLogic.GetType() != typeof(AOESupportAttack))
                        allNearbyEnemies.Add(enemy);
                }
            }

        }
        return allNearbyEnemies;
    }
    
    public override void EndAttack(InstancedData inIData)
    {
        List<GameObject> allConnectorLines = inIData.GetID<List<GameObject>>("AllConnectorLines");
        List<LazerLogic> allConnectorRenderers = inIData.GetID<List<LazerLogic>>("AllConnectorRenderers");
        foreach (GameObject connectorLine in allConnectorLines)
        {
            Destroy(connectorLine);
        }
        allConnectorLines.Clear();
        allConnectorRenderers.Clear();

        List<Entity> prevNearbyEnemies = inIData.GetID<List<Entity>>("AllNearbyEnemies");
        foreach (Entity enemy in prevNearbyEnemies)
        {
            var eem = enemy.GetComponent<EntityEffectManager>();
            eem.RemovePowerup(EffectToApply);
        }

        GameObject playerConnector = inIData.GetID<GameObject>("PlayerConnector");
        playerConnector.SetActive(false);
    }

}
