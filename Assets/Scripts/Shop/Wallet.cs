using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public int Coins = 0;
    public int AdditiveMultiplier = 1;
    public int SubtractiveMultiplier = 1;

    public void AddCoins(int amount)
    {
        Coins += amount * AdditiveMultiplier;
    }

    public bool TrySpendCoins(int amount)
    {
        if (Coins >= (amount * SubtractiveMultiplier))
        {
            Coins -= (amount * SubtractiveMultiplier);
            return true;
        }
        return false;
    }

    public void DeductCoins(int amount)
    {
        Coins -= amount;
        if (Coins < 0)
            Coins = 0;
    }
}
