using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ItemPickup : MonoBehaviour
{
    public bool InstantPickup = false;
    public bool OnlyPlayerPickup = false;
    public GameObject Prompt;
    public bool CanBePickedUp = true;
    public bool RequiresCost = false;
    public int ItemCost = 0;

    public UnityEvent OnItemPickup;
    private bool isInTrigger = false;
    private Collider2D otherCollider = null;

    private void Start()
    {
        if (Prompt != null)
        {
            Prompt.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && OnlyPlayerPickup || !CanBePickedUp)
            return;

        if (InstantPickup)
        {
            OnPickup(other.gameObject);
            Destroy(gameObject);
        }
        else
        {
            if (Prompt != null)
                Prompt.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && OnlyPlayerPickup || !CanBePickedUp)
            return;

        isInTrigger = true;
        otherCollider = other;

        if (RequiresCost)
        {
            Wallet wallet = other.GetComponent<Wallet>();
            if (wallet != null)
            {
                if (CanPickup(other.gameObject))
                {
                    if (wallet.TrySpendCoins(ItemCost))
                        CheckPickup(other.gameObject);
                }
            }
        }
        else
        {
            if (CanPickup(other.gameObject))
                CheckPickup(other.gameObject);
        }
    }

    private void Update()
    {
        if (isInTrigger)
        {
            if (RequiresCost)
            {
                Wallet wallet = otherCollider.GetComponent<Wallet>();
                if (wallet != null)
                {
                    if (CanPickup(otherCollider.gameObject))
                    {
                        if (wallet.TrySpendCoins(ItemCost))
                            CheckPickup(otherCollider.gameObject);
                    }
                }
            }
            else
            {
                if (CanPickup(otherCollider.gameObject))
                    CheckPickup(otherCollider.gameObject);
            }
        }
    }

    private void CheckPickup(GameObject other)
    {
         if (Prompt != null)
             Prompt.SetActive(false);
         
         OnPickup(other);
         Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && OnlyPlayerPickup || !CanBePickedUp)
            return;
        isInTrigger = false;
        if (Prompt != null)
            Prompt.SetActive(false);
    }

    public abstract void OnPickup(GameObject target);

    public abstract bool CanPickup(GameObject target);
}
