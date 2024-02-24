using UnityEngine;

public class Vine : MonoBehaviour
{
    public HingeJoint2D grabJoint;
    public GameObject grabSpot;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponentInParent<Frog>() != null)
        {
            col.gameObject.GetComponentInParent<Frog>().GrabVine(gameObject);
        }
    }
}
