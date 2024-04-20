using System;
using System.Collections;
using UnityEngine;

public class LeverWall : MonoBehaviour
{
    private Vector2 startSize;
    private Vector3 startPos;
    public bool retracting = false;
    private bool prevRetracting = false;
    
    public void ResetPos()
    {
        retracting = false;
        transform.position = startPos;
        GetComponent<SpriteRenderer>().size = startSize;
    }
    private void FixedUpdate()
    {
        if (startPos == Vector3.zero)
        {
            startPos = transform.position;
            startSize = GetComponent<SpriteRenderer>().size;
        }
        if (retracting)
        {
            float speed = 0.21f;
            transform.position += transform.up * (speed / 2);
            GetComponent<SpriteRenderer>().size += Vector2.down * speed;
            GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().size.x,
                Mathf.Max(GetComponent<SpriteRenderer>().size.y, 0));
        }

        prevRetracting = retracting;
    }
}
