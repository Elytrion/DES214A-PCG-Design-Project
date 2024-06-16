using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEncounterTrigger : MonoBehaviour
{
    [SerializeField]
    private BossEncounter _bossEncounter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _bossEncounter.StartEncounter();
        }
    }
}
