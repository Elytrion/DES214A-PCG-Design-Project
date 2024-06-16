using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLogic : MonoBehaviour
{
    public GameObject EffectPrefab;
    public int Damage = 0;
    public float LifeTime = 3.0f;
    private float _lifeTimer = -0.0f;

    private void Update()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= LifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Entity>().Health -= Damage;
            Instantiate(EffectPrefab, col.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
