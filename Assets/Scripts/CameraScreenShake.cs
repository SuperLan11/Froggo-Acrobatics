using UnityEngine;

public class CameraScreenShake : MonoBehaviour
{
    float screenShake = 0;
    Vector3 startPos;
    public AudioSource musicSource;
    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            screenShake += 20;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            screenShake += 10;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            AudioManager.instance.PlayCrash();
            AudioManager.instance.cutsceneMusicSource.Stop();
        }
        float intensity = screenShake / 10;
        transform.position = startPos + (Vector3)new Vector2(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));
    }

    void FixedUpdate()
    {
        screenShake = Mathf.Max(screenShake - 0.6f, 0);
    }
}
