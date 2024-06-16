using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    [SerializeField]
    private BossFightManager _bossFightManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _bossFightManager.StartFight();
            gameObject.SetActive(false);
        }
    }
}
