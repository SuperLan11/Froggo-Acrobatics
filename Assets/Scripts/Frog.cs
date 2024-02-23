using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Frog : MonoBehaviour
{
    public GameObject legTargets;
    enum State
    {
        Idle, Jumping, Airborne
    }

    private State state = State.Idle;
    void Start()
    {
        targetStartLocalPos = legTargets.transform.localPosition;
    }

    private bool leftPressed;
    private bool rightPressed;

    void Update()
    {
        if (state == State.Idle) {
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
            if (leftPressed && rb.velocity.x > -maxControllableHorizontalSpeed)
            {
                rb.AddForce(airtimeHorizontalMovementForce * Vector2.left);
            }
            if (rightPressed && rb.velocity.x < maxControllableHorizontalSpeed)
            {
                rb.AddForce(airtimeHorizontalMovementForce * Vector2.right);
            }
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
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        state = State.Idle;
        airtime = 0;
    }

    private Rigidbody2D rb => GetComponent<Rigidbody2D>();

    void Jump()
    {
        rb.AddForce(new Vector2(0, 500));
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
