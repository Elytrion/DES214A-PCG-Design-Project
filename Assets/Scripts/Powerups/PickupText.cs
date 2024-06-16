using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupText : MonoBehaviour
{
    public TMP_Text PowerupName;
    public GameObject Panel;
    public float DisplayTime = 1.5f;
    private float _timer = 0.0f;

    private void Start()
    {
        Panel.SetActive(false);
    }

    public void DisplayPickup(string text, float insetTime = -1.0f)
    {
        Panel.SetActive(true);
        PowerupName.text = text;
        _timer = (insetTime > 0) ? insetTime : DisplayTime;
    }

    private void Update()
    {
        if (_timer > 0.0f)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            Panel.SetActive(false);         
        }
    }
}
