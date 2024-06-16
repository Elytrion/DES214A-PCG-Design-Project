using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCursor : MonoBehaviour
{
    public Transform TargetTransform;
    public float mouseSensitivity = 0.5f;
    public float followSpeed = 5f;
    public float clampDistance = 3f;
    public Vector2 clampDistances = new Vector2(3f, 10f);
    public float jumpDistance = 0.01f;
    public Vector2Int cameraViewRange = new Vector2Int(4, 50);

    private bool zoomedOut = false;
    private Camera _cam;

    public bool StopFollowing = false;
    
    private void Start()
    {
        _cam = GetComponent<Camera>();
        zoomedOut = false;
    }

    void FixedUpdate()
    {
        if (StopFollowing)
            return;
        
        Vector3 targetPosition;

        // Check if the right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 playerPosition = TargetTransform.position;

            // Calculate the target position as the average of the player's position and the mouse position
            targetPosition = (playerPosition + mousePosition) / 2f;

            // Calculate the vector from the player to the target position
            Vector3 fromPlayerToTarget = targetPosition - playerPosition;

            // Clamp the distance of the target position from the player
            if (fromPlayerToTarget.magnitude > clampDistance)
            {
                targetPosition = playerPosition + fromPlayerToTarget.normalized * clampDistance;
            }
        }
        else
        {
            // If the right mouse button isn't pressed, target the player position
            targetPosition = TargetTransform.position;
        }

        // Ensure the target has the same z coordinate as the camera
        targetPosition.z = transform.position.z;

        // Interpolate between the camera's current position and the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < jumpDistance)
        {
            transform.position = targetPosition;
        }

        if (Input.GetKey(KeyCode.F2))
        {
            clampDistance = clampDistances.y;
          
            if (_cam.orthographicSize != cameraViewRange.y && !zoomedOut)
            {
                _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, cameraViewRange.y, Time.deltaTime * 5.0f);
                if (_cam.orthographicSize > cameraViewRange.y - 0.1f)
                {
                    _cam.orthographicSize = cameraViewRange.y;
                    zoomedOut = true;
                }
            }

            if (zoomedOut)
            {
                float scrollData = Input.GetAxis("Mouse ScrollWheel");
                _cam.orthographicSize -= scrollData * 20.0f;
                _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, clampDistances.x, clampDistances.y);
            }

        }
        else
        {
            zoomedOut = false;
            clampDistance = clampDistances.x;
            if (_cam.orthographicSize != cameraViewRange.x)
            {
                _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, cameraViewRange.x, Time.deltaTime * 5.0f);
                if (_cam.orthographicSize < cameraViewRange.x + 0.1f)
                {
                    _cam.orthographicSize = cameraViewRange.x;
                }
            }
        }
    }
}
