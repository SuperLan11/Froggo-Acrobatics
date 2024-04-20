using System;
using UnityEngine;

public class LeverWall : MonoBehaviour
{
    private Vector2 startSize;
    private Vector3 startPos;
    public bool retracting = false;
    public void Start()
    {
        startPos = transform.position;
        startSize = GetComponent<SpriteRenderer>().size;
    }
    public void ResetPos()
    {
        retracting = false;
        transform.position = startPos;
        GetComponent<SpriteRenderer>().size = startSize;
    }
    private void FixedUpdate()
    {
        if (retracting)
        {
            float speed = 0.21f;
            transform.position += transform.up * (speed / 2);
            GetComponent<SpriteRenderer>().size += Vector2.down * speed;
            GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().size.x,
                Mathf.Max(GetComponent<SpriteRenderer>().size.y, 0));
        }
    }
}
