using UnityEngine;

public class Frog : MonoBehaviour
{
    void Start()
    {
        Jump();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void Jump()
    {
        Debug.Log("Jumping!");
        transform.Translate(0, 10, 0);
    }
}
