using UnityEngine;

public class ForceNoLocalRotation : MonoBehaviour
{
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 180);
    }
}
