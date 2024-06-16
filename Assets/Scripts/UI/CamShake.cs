using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public static CamShake Instance = null;

    // Component references
    private Camera _camera = null;
    private Transform _transform = null;

    // Shake settings
    private float _duration = 0.0f;
    private float _magnitude = 0.0f;
    private float _maxDuration = 0.0f;

    public CameraFollowCursor _camFollow;

    // Awake is called before the first frame update
    public void Awake()
    {
        // Set the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }

        // Get the component references
        _camera = gameObject.GetComponent<Camera>();
        _transform = _camera.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the shake
        if (_duration > 0.0f)
        {
            // Update the timer
            _duration -= Time.deltaTime;

            // Calculate the shake offset
            Vector2 offset = Random.insideUnitCircle * _magnitude * (_duration / _maxDuration);

            // Apply the shake offset
            _transform.position = Vector3.Lerp(_transform.position, _transform.position + new Vector3(offset.x, offset.y, 0.0f), 0.5f * Time.deltaTime);
        }
        else
        {
            // Reset the shake
            _magnitude = 0.0f;
            _duration = 0.0f;
            //_camFollow.StopFollowing = false;
        }
    }

    // Shake the camera for a duration with a magnitude
    public void Shake(float inDuration, float inMagnitude)
    {
        // Set the shake settings
        _duration = inDuration;
        _maxDuration = inDuration;
        _magnitude = inMagnitude;
        //_camFollow.StopFollowing = true;
    }

    public void DamageShake()
    {
        Shake(0.2f, 25.0f);
    }
}

