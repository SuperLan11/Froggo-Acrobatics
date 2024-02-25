using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponentInParent<Frog>() != null)
        {
            other.gameObject.GetComponentInParent<Frog>().CollectCheckpoint(this);
        }
    }

    void Update()
    {
        bool active = !(Frog.instance.currentCheckpoint == null) && Frog.instance.currentCheckpoint.gameObject == gameObject;
        GetComponent<SpriteRenderer>().sprite = active ? activeSprite : inactiveSprite;
    }
}
