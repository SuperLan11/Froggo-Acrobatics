using System;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public GameObject follow;
    public Vector2 offset;
    public Vector2 currentOffset;
    void FixedUpdate()
    { 
        currentOffset = Vector2.Lerp(currentOffset, offset, Time.fixedDeltaTime);
        transform.position = Vector2.Lerp(transform.position, (Vector2)follow.transform.position + currentOffset, 0.2f);
        GetComponent<Collider2D>().offset = -currentOffset;
        transform.position += Vector3.back * 10;
    }
    
    public void Teleport(Vector2 position)
    {
        transform.position = (Vector3)position + Vector3.back * 10;
    }

    public void SetOffset(Vector2 offset)
    {
        this.offset = offset;
    }

    public void ResetOffset()
    {
        offset = Vector2.zero;
    }
}
