using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public GameObject camera;
    public float scale = 1f / 6;
    private Vector2 myStart;
    private Vector2 cameraStart;

    void Start()
    {
        myStart = transform.position;
        cameraStart = camera.transform.position;
    }
    void FixedUpdate()
    {
        Vector2 cameraDiff = (Vector2)camera.transform.position - cameraStart;
        Vector2 diff = cameraDiff * (1 - scale);
        transform.position = myStart + diff;
        transform.position += Vector3.forward;
    }
}
