using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool isGrounded;
    Timer _coyote;

    [SerializeField] private LayerMask WhatisGround;
    [SerializeField] private float coyoteTime = .1f;

    public event Action<bool> GroundedUpdated = delegate { };
    public bool IsGrounded
    {
        get => isGrounded; set
        {
            GroundedUpdated(value);
            if (!value && _coyote == null)
            {
                _coyote = Timer.Register(coyoteTime, () => EndCoyote());
                return;
            }
            if (value && _coyote != null)
                Timer.Cancel(_coyote);
            isGrounded = value;

        }
    }

    void EndCoyote()
    {
        isGrounded = false;
        _coyote = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        IsGrounded = other != null && (((1 << other.gameObject.layer) & WhatisGround) != 0);
    }
    private void OnTriggerStay(Collider other)
    {
        IsGrounded = other != null && (((1 << other.gameObject.layer) & WhatisGround) != 0);
    }
    private void OnTriggerExit(Collider other)
    {
        IsGrounded = false;
    }

}
