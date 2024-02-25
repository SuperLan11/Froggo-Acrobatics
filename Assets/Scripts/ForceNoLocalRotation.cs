using UnityEngine;

public class ForceNoLocalRotation : MonoBehaviour
{
    void Update()
    {
        transform.localRotation = Quaternion.identity;
    }
}
