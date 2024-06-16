using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public UnityEvent OnEntityDamage;

    public int Health = 1;
    public int MaxHealth = 1;

    public bool CanAttack = true;
    public bool CanMove = true;

    public bool NoDamage = false;

    public void ReduceHealth(int amt)
    {
        if (NoDamage)
            return;

        Health -= amt;

        OnEntityDamage.Invoke();
    }
}
