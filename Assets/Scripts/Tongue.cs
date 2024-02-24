using System;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    private float tileSize;
    public Vector2 start;
    public Vector2 end;

    public void Update()
    {
        float angle = Vector2.SignedAngle(Vector2.up, end - start);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position = (start + end) / 2;
        transform.localScale = new Vector3(1, (end - start).magnitude, 1);
    }
}
