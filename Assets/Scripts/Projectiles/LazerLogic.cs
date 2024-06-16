using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerLogic : MonoBehaviour
{
    private LineRenderer _lr;

    public int LazerDamage = 1;

    public LayerMask IgnoredLayers;

    public GameObject objectToSpawn;

    public Vector3 aimPos;

    private void Awake()
    {
        aimPos = Vector3.zero;
        _lr = GetComponent<LineRenderer>();
    }
    
    private void Update()
    {
        _lr.SetPosition(0, transform.position);
    }
    
    public void SetWidth(float width)
    {
        _lr.startWidth = width;
        _lr.endWidth = width;
    }

    public void SetColor(Color color)
    {
        _lr.startColor = color;
        _lr.endColor = color;
    }

    public void SetTarget(Vector3 target)
    {
        _lr.SetPosition(1, target);
        aimPos = target;
    }

    public void SpawnObject()
    {
        if (objectToSpawn != null)
        {
            Vector3 endPos = _lr.GetPosition(_lr.positionCount - 1);
            Instantiate(objectToSpawn, endPos, Quaternion.identity);
        }
    }

}
