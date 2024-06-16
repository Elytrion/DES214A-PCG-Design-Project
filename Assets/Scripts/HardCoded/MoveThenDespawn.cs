using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveThenDespawn : MonoBehaviour
{
    public bool StartMoving = false;
    public Vector2 EndPos;
    public float Speed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        if (StartMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, EndPos, Speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, EndPos) < 0.5f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void MoveTo(Vector2Int roomPt)
    {
        EndPos = new Vector2(roomPt.x, roomPt.y);
        StartMoving = true;
    }
}
