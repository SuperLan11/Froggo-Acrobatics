using System.Collections;
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
    public Tongue tonguePreview;
    public GameObject mouthSpot;

    public enum State
    {
        Idle,
        Jumping,
        Airborne,
        WallTethering,
        WallTethered,
        Landing,
        TongueGrappling,
        Hanging
    }

    public GameObject armSpot;
    public GameObject legSpot;
    private float armLegDistance;
    public static Frog instance;
    public float defaultTimeScale;
    public bool ceilingHanging;

    void Awake()
    {
        instance = this;
    }

    public State state = State.Idle;

    void Start()
    {
        targetStartLocalPos = legTargets.transform.localPosition;
        armLegDistance = Mathf.Abs(armSpot.transform.position.x - legSpot.transform.position.x);
        respawnPoint = transform.position;
        Time.timeScale = defaultTimeScale;
        gravityScale = rb.gravityScale;
        frontOldRotation = frontArm.transform.localRotation.eulerAngles.z;
        backOldRotation = backArm.transform.localRotation.eulerAngles.z;
    }

    private bool leftPressed;
    private bool rightPressed;

    IEnumerator AdjustFlipLater()
    {
        yield return new WaitForFixedUpdate();
        AdjustFlip();
    }

    public float tongueReachMultiplier = 2f;

    void Update()
    {
        if (state is State.Idle or State.WallTethered or State.TongueGrappling or State.Hanging)
        {
            //HHH
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        leftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        rightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        if (Input.GetMouseButtonDown(0) && state != State.TongueGrappling)
        {
            AudioManager.instance.PlayTongueShoot();
            tongueActive = true;
            Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 start = mouthSpot.transform.position;
            Vector2 diff = end - start;
            end = start + diff * tongueReachMultiplier;
            float m = Mathf.Min((end - start).magnitude, tongueMaxLength);
            end = start + diff.normalized * m;
            StartTongue(end);
        }

        if (tonguePreview != null)
        {
            UpdateTonguePreview();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TeleportToLevel(currentLevel);
        }
    }

    void UpdateTonguePreview()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos, LayerMask.NameToLayer("fwc"));
        tonguePreview.gameObject.SetActive(hit != null);
        if (hit == null)
        {
            return;
        }

        Debug.Log("hear");

        Vector2 start = mouthSpot.transform.position;
        Vector2 end = mousePos;
        Vector2 diff = end - start;
        if (diff.magnitude > tongueMaxLength)
        {
            end = start + diff.normalized * tongueMaxLength;
        }
        tonguePreview.start = start;
        tonguePreview.end = end;
        tonguePreview.Update();
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
        tongue.Update();
    }

    public float tongueProgressRate = 0.4f;
    public float tongueMaxLength = 6f;
    public float ceilingHangHorizontalJumpStrength = 0.2f;

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
        }
        else
        {
            nextPos = Vector2.MoveTowards(tongue.end, tongueTarget, tongueProgressRate);
        }

        tongue.start = mouthSpot.transform.position;
        tongue.end = nextPos;

        if (!tongueRetracting && Vector2.Distance(tongue.end, tongueTarget) < 0.1f ||
            Vector2.Distance(tongue.end, tongue.start) > tongueMaxLength)
        {
            tongueRetracting = true;
        }

        if (tongueRetracting && Vector2.Distance(tongue.end, mouthSpot.transform.position) < 0.1f)
        {
            EndTongue();
        }

        tongue.Update();
    }

    public void OnTongueCollide(GameObject other, Vector2 normal)
    {
        bool badState = state == State.TongueGrappling || prevState == State.TongueGrappling ||
                        state == State.WallTethering;

        
        if (!badState && (Utility.IsWall(other) || (normal == new Vector2(0, -1) && !ceilingHanging)) && !other.gameObject.CompareTag("NonClingable"))
        {
            if (state == State.WallTethered)
            {
                //Time.timeScale = 0.15f;
                if (currentTetherAttach != null) StartCoroutine(TemporarilyDisableCollision(currentTetherAttach.GetComponent<Collider2D>()));
            }

            if (state == State.Hanging)
            {
                UngrabVine();
            }
            state = State.TongueGrappling;
            left = other.transform.position.x < transform.position.x;
            StartCoroutine(AdjustFlipLater());
            UpdateTongue();
        }
        else
        {
            if (other.gameObject.CompareTag("NonClingable"))
            {
                AudioManager.instance.PlayTongueHitMetal();
            }
            tongueRetracting = true;
        }
    }

    private void EndTongue()
    {
        tongueActive = false;
    }

    private int airtime = 0;

    [FormerlySerializedAs("jumpLegAnimationTime")] [FormerlySerializedAs("JUMP_LEG_ANIMATION_TIME")]
    public int jumpingTime;

    private Vector3 targetStartLocalPos;
    public float maxControllableHorizontalSpeed;
    public float airtimeHorizontalMovementForce;
    public float vineSwingStrength;
    public float ceilingHangSwingStrength;

    private bool left = false;

    private void AdjustFlip()
    {
        float scaleX = left ? -1 : 1;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, 1);
        if (state == State.TongueGrappling)
        {
            UpdateTongue();
        }
    }

    private State prevState = State.Idle;
    public float autoRollThreshold = 90;
    public float autoRollTorque = 30;
    public float manualRollTorque = 15;
    public float maxRollSpeed = 400;
    public float tonguePullStrength = 10f;

    private void FixedUpdate()
    {
        if (state != State.Hanging)
        {
            ceilingHanging = false;
        }
        if (transform.position.y < DeathBarrier.instance.transform.position.y)
        {
            Die();
        }
        //Debug.Log(state.ToString());
        tongue.gameObject.SetActive(tongueActive);
        
        if (state == State.TongueGrappling)
        {
            rb.velocity = (tongue.end - (Vector2)transform.position).normalized * tonguePullStrength;
            Vector2 direction = (tongue.end - (Vector2)transform.position).normalized;
            float angle = Vector2.SignedAngle(left ? Vector2.left : Vector2.right, direction);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (tongueActive)
        {
            UpdateTongue();
        }

        if (state is State.Idle or State.WallTethered or State.Landing or State.WallTethering) //HHH
        {
            lastGrabbedVine = null;
            airtime = 0;
        }

        if (state == State.Hanging)
        {
            airtime = 0;
            /*if (!ceilingHanging)
            {
                float baseAngle = left ? -90 : 90;
                transform.rotation =
                    Quaternion.Euler(0, 0, lastGrabbedVine.transform.rotation.eulerAngles.z + baseAngle);
            }*/
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
            float strength = state == State.Airborne ? airtimeHorizontalMovementForce : ceilingHanging ? ceilingHangSwingStrength : vineSwingStrength;
            if (leftPressed && rb.velocity.x > -maxControllableHorizontalSpeed)
            {
                rb.AddForce(strength * Vector2.left);
            }

            if (rightPressed && rb.velocity.x < maxControllableHorizontalSpeed)
            {
                rb.AddForce(strength * Vector2.right);
            }
        }

        //HHH
        GetComponent<Collider2D>().enabled = state != State.WallTethering;
        rb.gravityScale = state is State.WallTethered or State.TongueGrappling ? 0 : gravityScale;
        rb.freezeRotation = state is State.WallTethered;// or State.Hanging;// && !ceilingHanging; // or State.TongueGrappling;
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
    private GameObject currentTetherAttach;
    public HingeJoint2D ceilingJoint;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("spikes"))
        {
            AudioManager.instance.PlaySpikeDeath();
            Die();
        }

        if (state == State.Hanging)
        {
            return;
        }

        Vector2 normal = col.GetContact(0).normal;
        bool ceilingNormal = normal == new Vector2(0, -1);
        bool floorNormal = normal == new Vector2(0, 1);
        bool rightmodeWallNormal = normal == new Vector2(-1, 0);
        bool leftmodeWallNormal = normal == new Vector2(1, 0);

        if (ceilingNormal && state == State.TongueGrappling)
        {
            if (ceilingHanging)
            {
                return;
            }
            ceilingJoint.enabled = true;
            ceilingHanging = true;
            Vector2 anchorPos = ceilingJoint.anchor + (Vector2)transform.position;
            Vector2 collisionPos = col.GetContact(0).point;
            Vector2 diff = collisionPos - anchorPos;
            transform.position -= (Vector3)diff*0.6f;
            state = State.Hanging;
            rb.angularVelocity = 0;
            ArmsHangPosition();
            EndTongue();
        } else if (ceilingNormal)
        {
            //do nothing
        }
        else if (Utility.IsFloor(col.gameObject) || (Utility.IsWall(col.gameObject) && floorNormal))
        {
            state = State.Landing;
            airtime = 0;
        }
        else if (Utility.IsWall(col.gameObject) && !col.gameObject.CompareTag("NonClingable") &&
                 state is State.Airborne or State.TongueGrappling)
        {
            //indicator.transform.position = col.GetContact(0).point;
            wallAttachPoint = col.GetContact(0).point;
            wallAttachPoint.y = armSpot.transform.position.y;
            if (leftmodeWallNormal)
            {
                left = true;
            } else if (rightmodeWallNormal)
            {
                left = false;
            }
            AdjustFlip();
            
            state = State.WallTethering;
            currentTetherAttach = col.gameObject;
            //indicator2.transform.position = col.GetContact(0).point + armLegDistance * Vector2.down;
            //transform.rotation = Quaternion.Euler(0, 0, 90);
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    private Rigidbody2D rb => GetComponent<Rigidbody2D>();

    public float hangingJumpStrengthUp = 350;
    public float ceilingJumpStrengthUp = -10;
    public float wallJumpStrengthUp = 250;
    public float wallJumpStrengthOut = 250;
    public float floorJumpStrength = 250;
    public float vineSpin;
    public float ceilingSpin;
    private Vector2 jumpVector;
    private float gravityScale = 0;

    public float initialJumpForceMultiplier = 1.5f;
    void SetupJump(Vector2 jumpVector)
    {
        this.jumpVector = jumpVector;
        rb.AddForce(jumpVector * initialJumpForceMultiplier);
    }

    void Jump()
    {
        if (state == State.TongueGrappling)
        {
            EndTongue();
            state = State.Airborne;
            return;
        }
        
        AudioManager.instance.PlayJump();
        if (state == State.Hanging)
        {
            UngrabVine();
            //rb.AddForce(new Vector2(0, hangingJumpStrengthUp));
            SetupJump(new Vector2(0, ceilingHanging ? ceilingJumpStrengthUp : hangingJumpStrengthUp));
            state = State.Jumping;
            rb.AddTorque(rb.velocity.x * (ceilingHanging ? ceilingSpin : vineSpin));

        }
        else
        {
            //rb.AddForce(new Vector2(0, wallJumpStrengthUp) + (Vector2)transform.up * 250);
            if (state is State.WallTethered or State.WallTethering)
            {
                SetupJump(new Vector2(0, wallJumpStrengthUp) + (Vector2)transform.up * wallJumpStrengthOut);
            }
            else
            {
                SetupJump(jumpVector = new Vector2(0, floorJumpStrength));
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
        legTargets.transform.localPosition =
            Vector3.Lerp(legTargets.transform.localPosition, targetStartLocalPos, instant ? 1 : 0.1f);
    }

    public GameObject lastGrabbedVine;

    public void GrabVine(GameObject vine)
    {
        Vine v = vine.GetComponent<Vine>();
        if (lastGrabbedVine == vine)
        {
            return;
        }
        transform.rotation = Quaternion.Euler(0, 0, left ? -90 : 90);
        ArmsHangPosition();

        v.grabJoint.connectedBody = rb;
        v.grabJoint.connectedAnchor = transform.InverseTransformPoint(armSpot.transform.position);
        state = State.Hanging;
        v.grabJoint.enabled = true;
        Vector3 difference = armSpot.transform.position - v.grabSpot.transform.position;
        transform.position -= difference;
        lastGrabbedVine = vine;
    }

    private void UngrabVine()
    {
        state = State.Airborne;
        if (lastGrabbedVine != null) lastGrabbedVine.GetComponent<Vine>().grabJoint.enabled = false;
        if (ceilingHanging)
        {
            rb.AddForce(Vector2.right * rb.angularVelocity * ceilingHangHorizontalJumpStrength);
        }
        ceilingJoint.enabled = false;
        ArmsResetPosition();
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
        Respawn(respawnPoint);
    }

    private void Die()
    {
        StartCoroutine(DieRoutine());
    }

    private IEnumerator TemporarilyDisableCollision(Collider2D other)
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, false);
    }

    public Checkpoint currentCheckpoint;
    public int currentLevel;
    public void CollectCheckpoint(Checkpoint c)
    {
        currentCheckpoint = c;
        respawnPoint = c.transform.position;
    }

    public void TeleportToLevel(int level)
    {
        respawnPoint = LevelStart.levelStarts[level].transform.position;
        Respawn(respawnPoint);
    }

    private IEnumerator EndOfLevelRoutine(int level)
    {
        Time.timeScale = 0f;
        AudioManager.instance.PlayClearStage();
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = defaultTimeScale;
        TeleportToLevel(level + 1);
    }

    public void EndOfLevel(int level)
    {
        StartCoroutine(EndOfLevelRoutine(level));
    }

    public GameObject frontArm;
    public GameObject backArm;
    private float frontOldRotation;
    private float backOldRotation;
    public void ArmsHangPosition()
    {
        frontArm.transform.localRotation = Quaternion.Euler(0, 0, -30);
        backArm.transform.localRotation = Quaternion.Euler(0, 0, -30);
    }

    public void ArmsResetPosition()
    {
        frontArm.transform.localRotation = Quaternion.Euler(0, 0, frontOldRotation);
        backArm.transform.localRotation = Quaternion.Euler(0, 0, backOldRotation);
    }
}
