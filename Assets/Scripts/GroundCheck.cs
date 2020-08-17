using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool isGrounded;
    [SerializeField] private LayerMask WhatisGround;
    public event Action<bool> GroundedUpdated = delegate { };
    public bool IsGrounded
    {
        get => isGrounded; set
        {
            isGrounded = value;
            GroundedUpdated(isGrounded);
        }
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
