using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public GameObject camera;
    public float scale = 1f / 6;
    private Vector2 myStart;
    private Vector2 cameraStart;
    public float speed = 0.01f;

    void Start()
    {
        myStart = transform.position;
        cameraStart = camera.transform.position;
    }
    public void FroggoFixedUpdate()
    {
        Vector2 cameraDiff = (Vector2)camera.transform.position - cameraStart;
        Vector2 diff = cameraDiff * (1 - scale);
        Vector2 goalPosition = myStart + diff;
        transform.position = goalPosition;
        transform.position += Vector3.forward;
    }
}
