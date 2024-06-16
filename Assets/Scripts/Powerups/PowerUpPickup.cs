using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPickup : ItemPickup
{
    public PowerUp PowerUpLogic;

    public override void OnPickup(GameObject target)
    {
        if (PowerUpLogic == null)
            return;
        
        OnItemPickup.Invoke();
        
        PickupText pickupPanel = FindObjectOfType<PickupText>();
        if (pickupPanel != null && PowerUpLogic.PickupText != null)
            pickupPanel.DisplayPickup(PowerUpLogic.PickupText);
        var EEM = target.GetComponent<EntityEffectManager>();
        if (EEM != null)
        {
            EEM.ApplyPowerUp(PowerUpLogic);
        }
    }

    public override bool CanPickup(GameObject target)
    {        
        return Input.GetKeyDown(KeyCode.E);
    }
}
