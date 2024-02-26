using System;
using UnityEngine;

public class LeverWall : MonoBehaviour
{
    private void FixedUpdate()
    {
        float speed = 0.07f;
        transform.position += Vector3.up * (speed / 2);
        GetComponent<SpriteRenderer>().size += Vector2.down * speed;
        GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().size.x,
            Mathf.Max(GetComponent<SpriteRenderer>().size.y, 0));
    }
}
