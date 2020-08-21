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
    [SerializeField] bool canDoubleJump = true;

    [Header("BETTER JUMP MODIFIERS")]

    [SerializeField] float fallMultiplier = 2.5f;


    [Header("GROUND CHECK")]

    [SerializeField] GroundCheck m_groundCheck;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask groundMask;


    [Header("SLIDING MECHANIC")]

    [SerializeField] Vector3 CapsuleCenter = new Vector3(-0.01134665f, 0.1278877f, -0.01678713f);
    [SerializeField] float CapsuleHeight = 0.2944223f;
    [SerializeField] float SlideTime = 55f;
    Vector3 HeadCasterDefaultPosition;


    [Header("WALL VAULTING")]
    [SerializeField] Transform[] BodyCasters;
    [SerializeField] Transform HeadCaster;
    [SerializeField] float RayDistance = 1;
    [SerializeField] float VaultForce = 1;

    [Header("WALL RUN")]
    [SerializeField] Transform WallRunChecker;
    [SerializeField] float WallRayDistance;

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

    bool isFalling = false;
    float jumpHeight;

    Rigidbody rb;

    CapsuleCollider CC;
    Vector3 defaultCCcenter;
    float defaultCCheight;
    Vector3 currCheckpoint;

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
        InputManager.instance.OnTap += Jump;
        InputManager.instance.OnHold += WallRun;
        InputManager.instance.OnSwipe += Slide;
        InputManager.instance.OnHoldEnd += WallRunEnd;
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
        for (int i = 0; i < 2; i++)
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
        rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    void Jump()
    {
        if (isDead || isWallRunning)
            return;
        if (IsGrounded)
        {
            RigidbodyJump();
            if (!canJumpAgain)
                canJumpAgain = true;
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
        for (int i = 0; i < 2; i++)
        {
            if (Physics.Raycast(BodyCasters[i].position, transform.forward, RayDistance, groundMask))
                onEdge = true;
            else
            {
                onEdge = false;
                break;
            }
        }
        RaycastHit hitInfo;
        bool OnHead = Physics.Raycast(HeadCaster.position, transform.forward, out hitInfo, RayDistance, groundMask) || Physics.Raycast(HeadCaster.position + new Vector3(0, .1f, 0), transform.forward, out hitInfo, RayDistance, groundMask);
        if (onEdge && !OnHead && !isGrounded)
            VaultJump();
        else if (onEdge && OnHead && IsGrounded)
            CollideDeath();
        else if (!onEdge && OnHead)
        {
            Death();
            //if (hitInfo.collider != null)
            //{
            //    Extensions.Debug(hitInfo.collider.transform.position.z.ToString());
            //    Extensions.Debug(HeadCaster.transform.position.z.ToString());
            //    Extensions.Debug(hitInfo.collider.gameObject.name);

            //}
        }

    }

    void CollideDeath()
    {
        //isDead = true;
        Death();
    }
    void VaultJump()
    {
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
        OnJump?.Invoke();
        //rb.AddForce(Vector3.up * 100 * jumpForce * Time.fixedDeltaTime);
        //WaitForFixedUpdate(() => rb.AddForce(Vector3.up * 100 * jumpForce * Time.fixedDeltaTime));
    }

    private void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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
        const float V = .25f;
        Timer.Register(TimeToRespawn + V, () => Revive());
        Timer.Register(TimeToRespawn + V + .1f, () => gameObject.SetActive(true));
        ResetVelocity();

    }

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
