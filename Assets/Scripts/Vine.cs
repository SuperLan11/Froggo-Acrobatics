using UnityEngine;

public class Vine : MonoBehaviour
{
    public HingeJoint2D hangJoint;
    public HingeJoint2D grabJoint;
    public GameObject grabSpot;
    private float initalMinAngle;
    private float initalMaxAngle;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponentInParent<Frog>() != null)
        {
            col.gameObject.GetComponentInParent<Frog>().GrabVine(gameObject);
        }
    }

    void Update()
    {
        bool frogPresent = Frog.instance.state == Frog.State.Hanging && Frog.instance.lastGrabbedVine == gameObject;
        if (!frogPresent) GetComponent<Rigidbody2D>().angularVelocity *= 0.8f;
    }

    private void Start()
    {
        grabJoint.anchor = new Vector2(0, -GetComponent<SpriteRenderer>().size.y / 2);
        hangJoint.anchor = new Vector2(0, GetComponent<SpriteRenderer>().size.y / 2);
        initalMinAngle = grabJoint.limits.min;
        initalMaxAngle = grabJoint.limits.max;
    }

    public void SetTipOffset(float angle)
    {
        var jointAngleLimits2D = grabJoint.limits;
        //jointAngleLimits2D.min = initalMinAngle - angle;
        //jointAngleLimits2D.max = initalMaxAngle - angle;
        grabJoint.limits = jointAngleLimits2D;
    }
}
