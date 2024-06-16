using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCoinDisplay : MonoBehaviour
{
    public TMP_Text CoinText;
    public Wallet WalletToTrack;

    void Update()
    {
        if (WalletToTrack != null)
        {
            CoinText.text = WalletToTrack.Coins.ToString();
        }
    }
}
