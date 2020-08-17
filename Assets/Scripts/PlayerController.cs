using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [Header("FALL DAMAGE DEBUG CALCS")]
    [SerializeField] Text VelocityTest;
    [SerializeField] float DeathHeight = -5f;
    #endregion

    #region Variables for basic storage and calculations
    bool isSliding = false;
    bool canJumpAgain = true;
    bool isDead = false;

    bool isFalling = false;
    float jumpHeight;

    Rigidbody rb;

    CapsuleCollider CC;
    Vector3 defaultCCcenter;
    float defaultCCheight;
    #endregion

    #region Events
    public event Action OnJump = delegate { };
    public event Action<float> OnLanded = delegate { };
    public event Action OnSlideBegin = delegate { };
    public event Action OnSlideEnd = delegate { };
    public event Action OnVault = delegate { };
    public event Action OnDeath = delegate { };
    public event Action OnWin = delegate { };

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
                VelocityTest.text = (jumpHeight - transform.position.y).ToString();
            }
            isGrounded = value;

        }
    }


    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CC = GetComponent<CapsuleCollider>();
        defaultCCcenter = CC.center;
        defaultCCheight = CC.height;
        HeadCasterDefaultPosition = HeadCaster.localPosition;
        InputManager.instance.OnTap += Jump;
        //InputManager.instance.OnHold += Jump;
        InputManager.instance.OnSwipe += Slide;
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
    }
    private void FixedUpdate()
    {
        if (isDead)
            return;
        AutoMoveForward();
        BetterJumpModifiers();
        CanVaultCheck();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (HeadCaster != null)
            Gizmos.DrawRay(HeadCaster.position, transform.forward * RayDistance);
        Gizmos.color = Color.green;
        for (int i = 0; i < 2; i++)
        {
            Gizmos.DrawRay(BodyCasters[i].position, transform.forward * RayDistance);
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
        if (isDead)
            return;
        if (IsGrounded)
            RigidbodyJump();
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
        bool OnHead = Physics.Raycast(HeadCaster.position, transform.forward, out hitInfo, RayDistance, groundMask);
        if (onEdge && !OnHead && !isGrounded)
            VaultJump();
        else if (onEdge && OnHead && IsGrounded)
            CollideDeath();
        else if (!onEdge && OnHead)
        {
            OnDeath();
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
        isDead = true;
        OnDeath();
    }
    void VaultJump()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.up * VaultForce * 100 * Time.fixedDeltaTime;
        Extensions.Debug("Vaulted");
        OnVault();
    }

    private void RigidbodyJump()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.up * jumpForce * 100 * Time.fixedDeltaTime;
        OnJump?.Invoke();
        //rb.AddForce(Vector3.up * 100 * jumpForce * Time.fixedDeltaTime);
        //WaitForFixedUpdate(() => rb.AddForce(Vector3.up * 100 * jumpForce * Time.fixedDeltaTime));
    }

    void Slide(SwipeData swipe)
    {
        if (isDead)
            return;
        if (swipe.Direction == SwipeDirection.Down)
            HeadCaster.localPosition = BodyCasters[0].localPosition;
        if (!isSliding && isGrounded)
        {
            isSliding = true;
            CC.center = CapsuleCenter;
            CC.height = CapsuleHeight;
            OnSlideBegin();
            Timer.Register(SlideTime, () => SlideEnd());
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

    public void Death()
    {
        OnDeath();
    }
    public void Win()
    {
        OnWin();
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
