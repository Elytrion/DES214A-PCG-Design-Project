using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : ItemPickup
{
    public int CoinValue = 1;
    
    private void Start()
    {
        InstantPickup = true; // Coins are always instant pickup
    }

    public override void OnPickup(GameObject target)
    {
        Wallet wallet = target.GetComponent<Wallet>();
        if (wallet != null)
        {
            wallet.AddCoins(CoinValue);
        }
    }

    public override bool CanPickup(GameObject target)
    {
        return true; // Always pick up coins
    }
}
