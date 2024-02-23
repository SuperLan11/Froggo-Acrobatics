using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Frog : MonoBehaviour
{
    public GameObject legTargets;
    enum State
    {
        Idle, Jumping, Airborne, WallTethering, WallTethered
    }

    public GameObject indicator;
    public GameObject indicator2;
    public GameObject armSpot;
    public GameObject legSpot;
    private float armLegDistance;

    private State state = State.Idle;
    void Start()
    {
        targetStartLocalPos = legTargets.transform.localPosition;
        armLegDistance = Mathf.Abs(armSpot.transform.position.x - legSpot.transform.position.x);
    }

    private bool leftPressed;
    private bool rightPressed;

    void Update()
    {
        if (state is State.Idle or State.WallTethered) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        
        leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    }

    private int airtime = 0;
    [FormerlySerializedAs("JUMP_LEG_ANIMATION_TIME")] public int jumpLegAnimationTime;
    private Vector3 targetStartLocalPos;
    public int maxControllableHorizontalSpeed;
    public int airtimeHorizontalMovementForce;

    private bool left = false;

    private void AdjustFlip()
    {
        float scaleX = left ? -1 : 1;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, 1);
    }
    private void FixedUpdate()
    {
        if (state == State.Idle)
        {
            if (leftPressed && !left)
            {
                left = true;
                AdjustFlip();
            }
            else if (rightPressed && left)
            {
                left = false;
                AdjustFlip();
            }
        }
        if (state == State.Jumping)
        {
            airtime++;
            if (airtime == jumpLegAnimationTime)
            {
                UnlockLegTargets();
                state = State.Airborne;
            }
        }

        if (state == State.Airborne)
        {
            MoveLegsTowardsStart();
        }

        if (state is State.Airborne or State.Jumping)
        {
            if (leftPressed && rb.velocity.x > -maxControllableHorizontalSpeed)
            {
                rb.AddForce(airtimeHorizontalMovementForce * Vector2.left);
            }
            if (rightPressed && rb.velocity.x < maxControllableHorizontalSpeed)
            {
                rb.AddForce(airtimeHorizontalMovementForce * Vector2.right);
            }
        }

        GetComponent<Collider2D>().enabled = state != State.WallTethering;
        rb.gravityScale = state == State.WallTethered ? 0 : 1;
        rb.freezeRotation = state == State.WallTethered;
        bool freezePosition = state == State.WallTethered;
        if (freezePosition)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezePosition;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
        }
        if (state == State.WallTethering)
        {
            Vector2 armDifference = wallAttachPoint - (Vector2)armSpot.transform.position;
            float moveMagnitude = Mathf.Min(armDifference.magnitude, 1f);
            float angleDifference = Mathf.Abs(transform.rotation.eulerAngles.z - 90);
            //Debug.Log(moveMagnitude + " " + angleDifference);
            
            
            Vector2 move = armDifference.normalized * moveMagnitude;
            transform.position += (Vector3)move;
            float z = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, 90, 6f);
            transform.rotation = Quaternion.Euler(0, 0, z);
            
            //Debug.Log(transform.position.x + " " + wallAttachPoint.x);
            //Debug.Log(Vector2.Distance(transform.position, (Vector3)wallAttachPoint));
            if (Vector3.Distance(armSpot.transform.position, (Vector3)wallAttachPoint) < 0.1f && angleDifference < 3f)
            {
                Debug.Log("Wall tethered!");
                state = State.WallTethered;
            }
        }
    }

    private Vector2 wallAttachPoint;
    private void OnCollisionEnter2D(Collision2D col)
    {
        state = State.Idle;
        airtime = 0;
        if (Utility.IsWall(col.gameObject))
        {
            //Time.timeScale = 0.2f;
            indicator.transform.position = col.GetContact(0).point;
            wallAttachPoint = col.GetContact(0).point;
            state = State.WallTethering;
            indicator2.transform.position = col.GetContact(0).point + armLegDistance * Vector2.down;
            //transform.rotation = Quaternion.Euler(0, 0, 90);
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    private Rigidbody2D rb => GetComponent<Rigidbody2D>();

    void Jump()
    {
        rb.AddForce(new Vector2(0, 250) + (Vector2)transform.up * 250);
        state = State.Jumping;
        LockLegTargets();
    }

    void LockLegTargets()
    {
        legTargets.transform.parent = transform.parent;
    }
    
    void UnlockLegTargets()
    {
        legTargets.transform.parent = transform;
    }

    void MoveLegsTowardsStart()
    {
        legTargets.transform.localPosition = Vector3.Lerp(legTargets.transform.localPosition, targetStartLocalPos, 0.1f);
    }
}
