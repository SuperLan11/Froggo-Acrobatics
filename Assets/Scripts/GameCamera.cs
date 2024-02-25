using System;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public GameObject follow;
    public Vector2 offset;
    public Vector2 currentOffset;
    void Update()
    { 
        currentOffset = Vector2.Lerp(currentOffset, offset, Time.deltaTime);
        transform.position = (Vector2)follow.transform.position + currentOffset;
        GetComponent<Collider2D>().offset = -currentOffset;
        transform.position += Vector3.back * 10;
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
