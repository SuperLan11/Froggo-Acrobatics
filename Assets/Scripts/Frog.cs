using System;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Frog : MonoBehaviour
{
    public GameObject legTargets;
    public Tongue tongue;
    public GameObject mouthSpot;
    enum State
    {
        Idle, Jumping, Airborne, WallTethering, WallTethered, Landing, TongueGrappling
    }

    public GameObject indicator;
    public GameObject indicator2;
    public GameObject armSpot;
    public GameObject legSpot;
    private float armLegDistance;
    public static Frog instance;

    void Awake()
    {
        instance = this;
    }

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
        if (state is State.Idle or State.WallTethered or State.TongueGrappling) { //HHH
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        //tongue.start = mouthSpot.transform.position;
        //tongue.end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        if (Input.GetMouseButtonDown(0))
        {
            tongueActive = true;
            StartTongue(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private bool tongueActive = false;
    public Vector2 tongueTarget;
    private bool tongueRetracting = false;

    private void StartTongue(Vector2 pos)
    {
        tongue.start = mouthSpot.transform.position;
        tongue.end = tongue.start;
        tongueActive = true;
        tongueTarget = pos;
        tongueRetracting = false;
    }

    private float tongueProgressRate = 0.4f;
    private float tongueMaxLength = 6f;
    private void UpdateTongue()
    {
        Vector2 nextPos;
        if (state == State.TongueGrappling)
        {
            nextPos = tongue.end;
        }
        else if (tongueRetracting)
        {
            nextPos = Vector2.MoveTowards(tongue.end, mouthSpot.transform.position, tongueProgressRate);
        } else {
            nextPos = Vector2.MoveTowards(tongue.end, tongueTarget, tongueProgressRate);
        }
        tongue.start = mouthSpot.transform.position;
        tongue.end = nextPos;
        
        if (!tongueRetracting && Vector2.Distance(tongue.end, tongueTarget) < 0.1f || Vector2.Distance(tongue.end, tongue.start) > tongueMaxLength)
        {
            tongueRetracting = true;
        }
        if (tongueRetracting && Vector2.Distance(tongue.end, mouthSpot.transform.position) < 0.1f)
        {
            EndTongue();
        }
    }

    public void OnTongueCollide(GameObject other)
    {
        Time.timeScale = 0.2f;
        if (state == State.TongueGrappling || prevState == State.TongueGrappling || state == State.WallTethering)
        {
            return;
        }
        Debug.Log("here");
        if (Utility.IsWall(other))
        {
            state = State.TongueGrappling;
            left = other.transform.position.x < transform.position.x;
        }
    }

    private void EndTongue()
    {
        tongueActive = false;
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

    private State prevState = State.Idle;
    private void FixedUpdate()
    {
        //Debug.Log(state.ToString());
        tongue.gameObject.SetActive(tongueActive);
        if (tongueActive)
        {
            UpdateTongue();
        }

        if (state == State.TongueGrappling)
        {
            rb.velocity = (tongue.end - (Vector2)transform.position).normalized * 10f;
            Vector2 direction = (tongue.end - (Vector2)transform.position).normalized;
            float angle = Vector2.SignedAngle(left ? Vector2.left : Vector2.right, direction);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        if (state is State.Idle or State.WallTethered or State.Landing) //HHH
        {
            airtime = 0;
        }

        if (state == State.Landing)
        {
            float angle = transform.rotation.eulerAngles.z;
            if (angle > 180)
            {
                angle -= 360;
            }

            if (rightPressed)
            {
                rb.AddTorque(-15);
            }
            else if (leftPressed)
            {
                rb.AddTorque(15);
            }
            else if (angle > 0 && Mathf.Abs(angle) > 90)
            {
                rb.AddTorque(-30);
            }
            else if (Mathf.Abs(angle) > 90)
            {
                rb.AddTorque(30);
            }
            if ((!rightPressed && !leftPressed && Mathf.Abs(angle) <= 90))
            {
                rb.angularVelocity *= 0.9f;
            }
            rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -400, 400);

            if (Mathf.Abs(angle) < 10 && rb.angularVelocity < 50f)
            {
                state = State.Idle;
                rb.angularVelocity = 0;
            }
        }
        
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

        if (state is State.Airborne or State.Idle or State.Landing)
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
        
        //HHH
        GetComponent<Collider2D>().enabled = state != State.WallTethering;
        rb.gravityScale = state is State.WallTethered or State.TongueGrappling ? 0 : 1;
        rb.freezeRotation = state is State.WallTethered;// or State.TongueGrappling;
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
            float targetAngle = left ? 270 : 90;
            float angleDifference = Mathf.Abs(transform.rotation.eulerAngles.z - targetAngle);
            //Debug.Log(moveMagnitude + " " + angleDifference);
            
            
            Vector2 move = armDifference.normalized * moveMagnitude;
            transform.position += (Vector3)move;
            float z = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, targetAngle, 6f);
            transform.rotation = Quaternion.Euler(0, 0, z);
            
            //Debug.Log(angleDifference);
            //Debug.Log(Vector2.Distance(transform.position, (Vector3)wallAttachPoint));
            if (Vector3.Distance(armSpot.transform.position, (Vector3)wallAttachPoint) < 0.1f && angleDifference < 3f)
            {
                Debug.Log("Wall tethered!");
                state = State.WallTethered;
            }
        }
        
        if (prevState == State.TongueGrappling && state != State.TongueGrappling)
        {
            EndTongue();
        }

        prevState = state;
    }

    private Vector2 wallAttachPoint;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (Utility.IsFloor(col.gameObject))
        {
            state = State.Landing;
            airtime = 0;
        }
        else if (Utility.IsWall(col.gameObject) && state is State.Airborne or State.TongueGrappling)
        {
            Debug.Log("MERP");
            //Time.timeScale = 0.2f;
            //indicator.transform.position = col.GetContact(0).point;
            wallAttachPoint = col.GetContact(0).point;
            wallAttachPoint.y = armSpot.transform.position.y;
            state = State.WallTethering;
            //indicator2.transform.position = col.GetContact(0).point + armLegDistance * Vector2.down;
            //transform.rotation = Quaternion.Euler(0, 0, 90);
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    private Rigidbody2D rb => GetComponent<Rigidbody2D>();

    void Jump()
    {
        if (state == State.TongueGrappling)
        {
            EndTongue();
            state = State.Airborne;
            return;
        }
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
