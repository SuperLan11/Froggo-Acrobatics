using System.Collections;
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
        Idle, Jumping, Airborne, WallTethering, WallTethered, Landing, TongueGrappling, Hanging
    }

    public GameObject armSpot;
    public GameObject legSpot;
    private float armLegDistance;
    public static Frog instance;
    public float defaultTimeScale;

    void Awake()
    {
        instance = this;
    }

    private State state = State.Idle;
    void Start()
    {
        targetStartLocalPos = legTargets.transform.localPosition;
        armLegDistance = Mathf.Abs(armSpot.transform.position.x - legSpot.transform.position.x);
        respawnPoint = transform.position;
        Time.timeScale = defaultTimeScale;
    }

    private bool leftPressed;
    private bool rightPressed;

    void Update()
    {
        if (state is State.Idle or State.WallTethered or State.TongueGrappling or State.Hanging) { //HHH
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

    public float tongueProgressRate = 0.4f;
    public float tongueMaxLength = 6f;
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
        //Time.timeScale = 0.2f;
        if (state == State.TongueGrappling || prevState == State.TongueGrappling || state == State.WallTethering || state == State.Hanging)
        {
            return;
        }
        
        Debug.Log("here");
        if (Utility.IsWall(other) && !other.gameObject.CompareTag("NonClingable"))
        {
            state = State.TongueGrappling;
            left = other.transform.position.x < transform.position.x;
            AdjustFlip();
        }
        else
        {
            tongueRetracting = true;
        }
    }

    private void EndTongue()
    {
        tongueActive = false;
    }

    private int airtime = 0;
    [FormerlySerializedAs("jumpLegAnimationTime")] [FormerlySerializedAs("JUMP_LEG_ANIMATION_TIME")] public int jumpingTime;
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
    public float autoRollThreshold = 90;
    public float autoRollTorque = 30;
    public float manualRollTorque = 15;
    public float maxRollSpeed = 400;
    public float tonguePullStrength = 10f;
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
            rb.velocity = (tongue.end - (Vector2)transform.position).normalized * tonguePullStrength;
            Vector2 direction = (tongue.end - (Vector2)transform.position).normalized;
            float angle = Vector2.SignedAngle(left ? Vector2.left : Vector2.right, direction);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        if (state is State.Idle or State.WallTethered or State.Landing or State.Hanging or State.WallTethering) //HHH
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
                rb.AddTorque(-manualRollTorque);
            }
            else if (leftPressed)
            {
                rb.AddTorque(manualRollTorque);
            }
            else if (angle > 0 && Mathf.Abs(angle) > autoRollThreshold)
            {
                rb.AddTorque(-autoRollTorque);
            }
            else if (Mathf.Abs(angle) > autoRollThreshold)
            {
                rb.AddTorque(autoRollTorque);
            }
            //if ((!rightPressed && !leftPressed && Mathf.Abs(angle) <= 90))
            //{
            //    rb.angularVelocity *= 0.9f;
            //}
            rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxRollSpeed, maxRollSpeed);

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
            if (airtime == jumpingTime)
            {
                UnlockLegTargets();
                state = State.Airborne;
            }
            
            if (Input.GetKey(KeyCode.Space))
            {
                rb.AddForce(jumpVector * Mathf.Pow(0.9f, airtime));
            }
        }

        if (state is State.Airborne or State.Idle or State.Landing or State.Hanging) //HHH
        {
            MoveLegsTowardsStart();
        }

        if (state is State.Airborne or State.Jumping or State.Hanging) //HHH
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
        if (col.gameObject.layer == LayerMask.NameToLayer("spikes"))
        {
            Die();
        }
        if (Utility.IsFloor(col.gameObject))
        {
            state = State.Landing;
            airtime = 0;
        }
        else if (Utility.IsWall(col.gameObject) && !col.gameObject.CompareTag("NonClingable") && state is State.Airborne or State.TongueGrappling)
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
    
    public float hangingJumpStrengthUp = 350;
    public float wallJumpStrengthUp = 250;
    public float wallJumpStrengthOut = 250;
    public float floorJumpStrength = 250;
    public float vineSpin;
    private Vector2 jumpVector;
    void Jump()
    {
        if (state == State.TongueGrappling)
        {
            EndTongue();
            state = State.Airborne;
            return;
        }

        if (state == State.Hanging)
        {
            UngrabVine();
            //rb.AddForce(new Vector2(0, hangingJumpStrengthUp));
            jumpVector = new Vector2(0, hangingJumpStrengthUp);
            state = State.Jumping;
            rb.AddTorque(rb.velocity.x * vineSpin);
            
        }
        else
        {
            //rb.AddForce(new Vector2(0, wallJumpStrengthUp) + (Vector2)transform.up * 250);
            if (state is State.WallTethered or State.WallTethering)
            {
                jumpVector = new Vector2(0, wallJumpStrengthUp) + (Vector2)transform.up * wallJumpStrengthOut;
            }
            else
            {
                jumpVector = new Vector2(0, floorJumpStrength);
            }
            state = State.Jumping;
            LockLegTargets();
        }
        
    }

    void LockLegTargets()
    {
        legTargets.transform.parent = transform.parent;
    }
    
    void UnlockLegTargets()
    {
        legTargets.transform.parent = transform;
    }

    void MoveLegsTowardsStart(bool instant = false)
    {
        legTargets.transform.localPosition = Vector3.Lerp(legTargets.transform.localPosition, targetStartLocalPos, instant ? 1 : 0.1f);
    }
    
    private Vine lastGrabbedVine;
    public void GrabVine(GameObject vine)
    {
        Vine v = vine.GetComponent<Vine>();
        v.grabJoint.connectedBody = rb;
        v.grabJoint.connectedAnchor = transform.InverseTransformPoint(armSpot.transform.position);
        state = State.Hanging;
        v.grabJoint.enabled = true;
        Vector3 difference = armSpot.transform.position - v.grabSpot.transform.position;
        transform.position -= difference;
        lastGrabbedVine = v;
    } 
    
    private void UngrabVine()
    {
        state = State.Airborne;
        lastGrabbedVine.grabJoint.enabled = false;
    }

    private Vector2 respawnPoint;

    private void Respawn(Vector2 pos)
    {
        transform.position = pos;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.constraints = RigidbodyConstraints2D.None;
        transform.rotation = Quaternion.identity;
        if (state == State.Hanging)
        {
            UngrabVine();
        }
        EndTongue();
        state = State.Idle;
        MoveLegsTowardsStart(true);
    }

    IEnumerator DieRoutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = defaultTimeScale;
        Debug.Log("respawning");
        Respawn(respawnPoint);
    }

    private void Die()
    {
        StartCoroutine(DieRoutine());
    }
}
