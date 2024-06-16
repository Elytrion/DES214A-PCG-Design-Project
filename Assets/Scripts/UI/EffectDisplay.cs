using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDisplay : MonoBehaviour
{
    [SerializeField]
    private EntityEffectManager _playerEEM;

    [SerializeField]
    private GameObject powerUpIconPrefab; // Prefab of a UI element representing a single power-up

    [SerializeField]
    private Transform powerUpIconContainer; // The GameObject with the Horizontal Layout Group and Content Size Fitter attached

    // A dictionary of power-up icons, indexed by the power-up ID
    private Dictionary<string, GameObject> powerUpIcons = new Dictionary<string, GameObject>();

    private void Update()
    {
        // Get a copy of the power-up dictionary keys
        List<string> keys = new List<string>(_playerEEM.ActivePowerUps.Keys);

        // For each power-up in the manager
        foreach (string key in keys)
        {
            if (!powerUpIcons.ContainsKey(key))
            {
                if (_playerEEM.ActiveEffects[key].isInstant)
                {
                    continue; // ignore instant effects
                }

                // This power-up has no display yet, create one
                GameObject newIcon = Instantiate(powerUpIconPrefab, powerUpIconContainer);
                powerUpIcons[key] = newIcon;
                EffectIconDisplay iconDis = newIcon.GetComponent<EffectIconDisplay>();
                iconDis.SetIcon(_playerEEM.ActiveEffects[key].PowerupSprite);
            }

            EffectIconDisplay iconDisplay = powerUpIcons[key].GetComponent<EffectIconDisplay>();
            iconDisplay.SetText(_playerEEM.ActivePowerUps[key].ToString("F2"));
        }

        // Remove any displays for power - ups that aren't active

        List<string> iconKeys = new List<string>(powerUpIcons.Keys);
        List<string> keysToRemove = new List<string>();
        foreach (string key in iconKeys)
        {
            if (!_playerEEM.ActivePowerUps.ContainsKey(key))
            {
                Destroy(powerUpIcons[key]);
                keysToRemove.Add(key);
            }
        }

        foreach (string key in keysToRemove)
        {
            powerUpIcons.Remove(key);
        }
    }
}
