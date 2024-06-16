using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEntityOnHit : MonoBehaviour
{
    // Shake parameters
    public float duration = 0.5f;
    public float magnitude = 0.2f;

    // Internal shake tracking
    private Vector3 originalPos;
    private float shakeTimeRemaining = 0;

    public bool useGlobalPosition = false;

    // Call this to start the shake
    public void TriggerShake()
    {
        originalPos = (useGlobalPosition) ? this.transform.position : this.transform.localPosition;
        shakeTimeRemaining = duration;
    }

    void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            float dampingFactor = shakeTimeRemaining / duration;
            float x = Random.Range(-1f, 1f) * magnitude * dampingFactor;
            float y = Random.Range(-1f, 1f) * magnitude * dampingFactor;

            if (useGlobalPosition)
                transform.position = new Vector3(x, y, originalPos.z);
            else
                transform.localPosition = new Vector3(x, y, originalPos.z);

            shakeTimeRemaining -= Time.deltaTime;

            if (shakeTimeRemaining <= 0)
            {
                shakeTimeRemaining = 0;
                if (useGlobalPosition)
                    transform.position = originalPos;
                else
                    transform.localPosition = originalPos;
            }
        }
    }
}
