using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    public static DeathBarrier instance;
    void Awake()
    {
        instance = this;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
