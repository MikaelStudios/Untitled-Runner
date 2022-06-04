using DG.Tweening;
using Hellmade.Sound;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    #region Serializable Fields for the Inspector
    [Header("MOVEMENT PROPERTIES")]

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float maxSpeed;
    [SerializeField] bool canDoubleJump = true;

    [Header("BETTER JUMP MODIFIERS")]

    [SerializeField] float fallMultiplier = 2.5f;


    [Header("GROUND CHECK")]

    [SerializeField] GroundCheck m_groundCheck;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask groundMask;


    [Header("CLIMBING MECHANICS")]
    [SerializeField] bool isClimbing = false;
    [SerializeField] float climbSpeed = 3.5f;


    [Header("SLIDING MECHANIC")]

    [SerializeField] Vector3 CapsuleCenter = new Vector3(-0.01134665f, 0.1278877f, -0.01678713f);
    [SerializeField] float CapsuleHeight = 0.2944223f;
    [SerializeField] float SlideTime = 55f;
    Vector3 HeadCasterDefaultPosition;

    [Header("LANE CHANGING")]
    [SerializeField] bool canChangeLane;
    [SerializeField] float laneChangeSpeed;

    [Header("WALL VAULTING")]
    [SerializeField] Transform[] BodyCasters;
    [SerializeField] Transform HeadCaster;
    [SerializeField] float RayDistance = 1;
    [SerializeField] float VaultForce = 1;

    [Header("WALL RUN")]
    [SerializeField] Transform WallRunChecker;
    [SerializeField] float WallRayDistance;

    [Header("POWER UP")]
    [SerializeField] float speedBoostTime;
    public bool speedBoostDefence = false;

    [Header("FALL DAMAGE DEBUG CALCS")]
    [SerializeField] float DeathHeight = -5f;

    [Header("HEALTH AND DEATH PARAMETERS")]
    [SerializeField] bool InstantDeath = false;
    [SerializeField] float TimeToRespawn = 1;
    [SerializeField] float SlowDownTime = 1;

    [Header("AUDIO")]
    [SerializeField] AudioClip[] Sound_Jump;
    [SerializeField] AudioClip[] Sound_Vault;
    [SerializeField] AudioClip Sound_Slide;
    #endregion

    #region Variables for basic storage and calculations
    bool isSliding = false;
    bool canJumpAgain = true;
    bool isDead = false;
    bool isWallRunning = false;
    bool isBoosting = false;
    bool isVaulting = false;

    bool isFalling = false;
    float jumpHeight;

    Rigidbody rb;

    CapsuleCollider CC;
    Vector3 defaultCCcenter;
    float defaultCCheight;
    Vector3 currCheckpoint;
    int no_Casters;
    #endregion

    #region Events
    public event Action OnJump = delegate { };
    public event Action<float> OnLanded = delegate { };
    public event Action OnWallRun = delegate { };
    public event Action OnWallRunEnd = delegate { };
    public event Action OnSlideBegin = delegate { };
    public event Action OnSlideEnd = delegate { };
    public event Action OnVault = delegate { };
    public event Action<bool> OnDeath = delegate { };
    public event Action OnWin = delegate { };
    public event Action AirBoost = delegate { };
    public event Action Stumble = delegate { };
    public event Action OnClimbStart = delegate { };
    public event Action OnClimbEnd = delegate { };
    #endregion

    #region Properties

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            if (value && !isGrounded)
            {
                canJumpAgain = true;
                OnLanded(jumpHeight - transform.position.y);
                isFalling = false;
                isBoosting = false;
                isVaulting = false;
                //VelocityTest.text = (jumpHeight - transform.position.y).ToString();
            }
            isGrounded = value;

        }
    }

    public float RespawnTime { get => TimeToRespawn; /*set => TimeToRespawn = value;*/ }


    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CC = GetComponent<CapsuleCollider>();
        defaultCCcenter = CC.center;
        defaultCCheight = CC.height;
        HeadCasterDefaultPosition = HeadCaster.localPosition;
        currCheckpoint = transform.position;
        no_Casters = BodyCasters.Length;
        InputManager.instance.OnTap += Jump;
        InputManager.instance.OnHold += WallRun;
        InputManager.instance.OnSwipe += Slide;
        //InputManager.instance.OnSwipe += LaneChange;
        InputManager.instance.OnHoldEnd += WallRunEnd;
        if (!name.Equals("Generic Girl"))
            ReassignCasters();
        //m_groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private void Update()
    {
        if (isDead)
            return;
        IsGrounded = m_groundCheck.IsGrounded;
        if (!isFalling && rb.velocity.y < 0)
        {
            jumpHeight = transform.position.y;
            isFalling = true;
        }
        if (transform.position.y < DeathHeight)
            Death();
        CanVaultCheck();
        //transform.position = transform.position.SetPositionX(Mathf.Clamp(transform.position.x, -1, 0));
    }

    public void ReassignCasters()
    {
        m_groundCheck = GetComponentInChildren<GroundCheck>();
        BodyCasters[0] = transform.Find("Body Ray Caster(Clone)").transform;
        BodyCasters[1] = BodyCasters[0].Find("Body Ray Caster").transform;
        WallRunChecker = transform.Find("Wall RUn Ray Caster(Clone)").transform;
        HeadCaster = transform.Find("HeadRay Caster(Clone)").transform;
    }
    private void FixedUpdate()
    {
        if (isDead)
            return;
        AutoMoveForward();
        BetterJumpModifiers();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (HeadCaster != null)
        {
            Gizmos.DrawRay(HeadCaster.position, transform.forward * RayDistance);
            Gizmos.DrawRay(HeadCaster.position + new Vector3(0, .1f, 0), transform.forward * RayDistance);
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < BodyCasters.Length; i++)
        {
            Gizmos.DrawRay(BodyCasters[i].position, transform.forward * RayDistance);
        }
        Gizmos.color = Color.blue;
        if (WallRunChecker != null)
        {
            Gizmos.DrawRay(WallRunChecker.position, -transform.right * WallRayDistance);
        }
    }

    bool stumble = false;
    bool slowdownNormal = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("SlowDown"))
        {
            if (!stumble)
            {
                Speed /= 2;
                stumble = true;

                if (collision.collider.gameObject.HasRigidbody())
                {

                    Stumble();
                    Timer.Register(SlowDownTime, () => Speed *= 2);
                    Timer.Register(SlowDownTime, () => stumble = false);
                }
                else
                    slowdownNormal = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("SlowDown") && stumble && slowdownNormal)
        {
            Speed *= 2;
            stumble = false;
            slowdownNormal = false;
        }
    }

    private void BetterJumpModifiers()
    {
        if (rb.velocity.y > 0)
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
    }

    private void AutoMoveForward()
    {
        if (isClimbing)
            return;
        rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
        if (speed < maxSpeed)
            speed += Time.deltaTime * speedMultiplier;
    }

    void Jump()
    {
        if (isDead || isWallRunning || isClimbing)
            return;
        if (IsGrounded)
        {
            RigidbodyJump();
            if (!canJumpAgain)
                canJumpAgain = true;
            OnJump();
        }
        else if (canJumpAgain && canDoubleJump)
        {
            RigidbodyJump();
            canJumpAgain = false;
            isFalling = false;
        }
    }

    private void CanVaultCheck()
    {
        bool onEdge = false;
        for (int i = no_Casters - 1; i >= 0; i--)
        {
            if (Physics.Raycast(BodyCasters[i].position, transform.forward, RayDistance, groundMask))
                onEdge = true;
            else
            {
                if (onEdge == true)
                {
                    rb.ResetVelocity();
                    rb.velocity = Vector3.up * (VaultForce / 2) * 100 * Time.fixedDeltaTime;
                }
                onEdge = false;
                break;
            }
        }
        RaycastHit hitInfo;
        bool OnHead = Physics.Raycast(HeadCaster.position, transform.forward, out hitInfo, RayDistance, groundMask) || Physics.Raycast(HeadCaster.position + new Vector3(0, .1f, 0), transform.forward, out hitInfo, RayDistance, groundMask);
        if (onEdge && !OnHead && !isGrounded)
            VaultJump();
        else if (onEdge || OnHead)
            ClimbUpWards();
        else if (isClimbing)
        {
            isClimbing = false;
            rb.useGravity = true;
            isVaulting = false;
            OnClimbEnd();
        }
        //if (speedBoostDefence)
        //    return;
        //else if (onEdge && OnHead && IsGrounded)
        //    CollideDeath();
        //else if (!onEdge && OnHead)
        //{
        //    Death();
        //}

    }

    void CollideDeath()
    {
        //isDead = true;
        Death();
    }
    void ClimbUpWards()
    {
        if (!isClimbing)
        {
            isClimbing = true;
            rb.ResetVelocity();
            rb.useGravity = false;
            OnClimbStart();
            isVaulting = false;
        }
        //rb.useGravity =
        //WaitForFixedUpdate(() => rb.MovePosition(transform.position + transform.up * Time.fixedDeltaTime * speed));
        rb.MovePosition(transform.position + Vector3.up * Time.fixedDeltaTime * climbSpeed);
    }
    void VaultJump()
    {
        if (isVaulting)
            return;
        isVaulting = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.up * VaultForce * 100 * Time.fixedDeltaTime;
        if (isWallRunning)
        {
            OnWallRunEnd();
        }
        EazySoundManager.PlaySound(Sound_Vault.GetRandom());
        Extensions.Debug("Vaulted");
        OnVault();
    }

    void WallRun()
    {
        if (isDead)
            return;
        if (Physics.Raycast(WallRunChecker.position, -transform.right, WallRayDistance, groundMask))
        {
            if (!isWallRunning)
            {
                canJumpAgain = true;
                OnWallRun();
                isWallRunning = true;
                rb.isKinematic = true;
            }

        }
        else
        {

            WallRunEnd();
        }
    }
    void WallRunEnd()
    {
        if (isWallRunning)
        {
            OnWallRunEnd();
            rb.isKinematic = false;
            ResetVelocity();
            isWallRunning = false;
        }
    }
    private void RigidbodyJump()
    {
        if (!isBoosting)
            ResetVelocity();
        //rb.velocity = Vector3.up * jumpForce * 100 * Time.fixedDeltaTime;
        EazySoundManager.PlaySound(Sound_Jump.GetRandom());
        rb.AddForce(Vector3.up * jumpForce * 100 * Time.fixedDeltaTime, ForceMode.Impulse);

        //rb.AddForce(Vector3.up * 100 * jumpForce * Time.fixedDeltaTime);
        //WaitForFixedUpdate(() => rb.AddForce(Vector3.up * 100 * jumpForce * Time.fixedDeltaTime));
    }

    private void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void LaneChange(SwipeData swipe)
    {
        if (!canChangeLane)
            return;
        if (swipe.Direction == SwipeDirection.Left && isInRangeOf(transform.position.x, 0))
            transform.DOMoveX(-1, laneChangeSpeed);
        if (swipe.Direction == SwipeDirection.Right && isInRangeOf(transform.position.x, -1))
            transform.DOMoveX(0, laneChangeSpeed);

    }
    bool isInRangeOf(float a, float x)
    {

        if (a <= x + .01f && a >= x - .01f)
            return true;
        else
            return false;

    }
    void Slide(SwipeData swipe)
    {
        if (isDead || isWallRunning)
            return;
        if (swipe.Direction == SwipeDirection.Down)
        {
            HeadCaster.localPosition = BodyCasters[0].localPosition;
            if (!isSliding && isGrounded)
            {
                isSliding = true;
                CC.center = CapsuleCenter;
                CC.height = CapsuleHeight;
                OnSlideBegin();
                Timer.Register(SlideTime, () => SlideEnd());
                EazySoundManager.PlaySound(Sound_Slide);
            }
        }
    }

    void SlideEnd()
    {
        OnSlideEnd();
        HeadCaster.localPosition = HeadCasterDefaultPosition;
        CC.center = defaultCCcenter;
        CC.height = defaultCCheight;
        isSliding = false;
    }

    public void AddCheckPoint(Vector3 C)
    {
        if (C.z > currCheckpoint.z)
            currCheckpoint = C;
    }

    public void Death()
    {
        isDead = true;
        OnDeath(InstantDeath);
        if (InstantDeath)
            return;
        Timer.Register(TimeToRespawn, () => transform.position = currCheckpoint);
        const float V = .35f;
        Timer.Register(TimeToRespawn + V, () => Revive());
        Timer.Register(TimeToRespawn + V + .1f, () => gameObject.SetActive(true));
        ResetVelocity();

    }

    Timer _SD;
    public void SpeedBoost(Vector3 force, float addedSpeed)
    {
        if (force.y != 0)
        {
            AirBoost();
        }
        if (!isBoosting)
        {
            isBoosting = true;
            rb.AddForce(force * 100 * Time.fixedDeltaTime, ForceMode.Impulse);
        }
        Speed += addedSpeed;
        speedBoostDefence = true;
        if (_SD != null)
            _SD.Cancel();
        _SD = Timer.Register(speedBoostTime, () => speedBoostDefence = false);
    }
    public void Revive()
    {
        isDead = false;
        isFalling = false;
    }
    public void Win()
    {
        OnWin();
        InputManager.instance.OnTap -= Jump;
        //InputManager.instance.OnHold += Jump;
        InputManager.instance.OnSwipe -= Slide;
    }

    #region Helper Methods
    void WaitForFixedUpdate(UnityAction action)
    {
        StartCoroutine(I_WaitForFixedUpdate(action));
    }
    IEnumerator I_WaitForFixedUpdate(UnityAction action)
    {
        yield return new WaitForFixedUpdate();
        action.Invoke();
    }
    #endregion

}
