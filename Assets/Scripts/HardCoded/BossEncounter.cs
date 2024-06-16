using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEncounter : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _bossToSpawn;

    [SerializeField]
    private GameObject _blockingWalls;

    private bool _bossEncounterStarted = false;

    private Entity _selectedBoss;

    private void Start()
    {
        foreach (var boss in _bossToSpawn)
        {
            boss.SetActive(false);
        }
        _blockingWalls.SetActive(false);
    }

    public void StartEncounter()
    {
        if (_bossEncounterStarted)
            return;

        _bossEncounterStarted = true;

        _blockingWalls.SetActive(true);

        GameObject bossToSpawn = _bossToSpawn[Random.Range(0, _bossToSpawn.Length)];
        bossToSpawn.SetActive(true);
        _selectedBoss = bossToSpawn.GetComponent<EnemyBase>()._entity;
    }

    private void Update()
    {
        if (_bossEncounterStarted)
        {
            if (_selectedBoss.Health <= 0)
            {
                _blockingWalls.SetActive(false);
            }
        }
    }
}
