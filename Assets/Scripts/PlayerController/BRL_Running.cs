using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Running : BRL_BaseState
{
    private Vector2 _moveInput;
    // Start is called before the first frame update

    public override void OnEnter(BR_PlayerController br_pc)
    {
        base.OnEnter(br_pc);
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        _moveInput.x = Input.GetAxis("Horizontal");
        _moveInput.y = Input.GetAxis("Vertical");
        _moveInput.Normalize();

        Rigidbody _rb = pc.GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(_moveInput.x * pc.getMoveSpeed(), _rb.velocity.y, _moveInput.y * pc.getMoveSpeed());

        pc.spriteAnimator.SetFloat("moveSpeed", _rb.velocity.magnitude);

        RaycastHit hit;

        bool _isGrounded;

        if (Physics.Raycast(pc.groundPoint.position, Vector3.down, out hit, .3f, pc.groundLayer))
        {
            _isGrounded = true;
        }
        else _isGrounded = false;

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            //_rb.velocity += new Vector3(0f, jumpForce, 0f);
            // transition to Jump State
            pc.changeState(pc.Running);
        }

        pc.spriteAnimator.SetBool("onGround", _isGrounded);

        if (!pc._isFlipped && _moveInput.x < 0)
        {
            pc._isFlipped = true;
            //_flipX = true; // todo: this should be made observable somehow
            //_spriteRenderer.flipX = _flipX;
            pc.spriteAnimator.SetBool("swapX", true);
            pc._spriteRenderer.flipX = pc.spriteAnimator.GetBool("swapX");
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        } else if (pc._isFlipped && _moveInput.x > 0)
        {
            pc._isFlipped = false;
            //_flipX = false;
            //_spriteRenderer.flipX = _flipX;
            pc.spriteAnimator.SetBool("swapX", false);
            pc._spriteRenderer.flipX = pc.spriteAnimator.GetBool("swapX");
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        }

        if (!pc._isBackwards && _moveInput.y > 0)
        {
            pc._isBackwards = true;
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        } else if (pc._isBackwards && _moveInput.y < 0)
        {
            pc._isBackwards = false;
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        }

        pc.spriteAnimator.SetBool("movingBackwards", pc._isBackwards);
    }
}
