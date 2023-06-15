using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_BaseState
{
    protected BR_PlayerController pc; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public BR_PlayerController.PlayerState stateName;

    public virtual void OnUpdate() {
        pc._moveInput = pc.movement.action.ReadValue<Vector2>();
        //pc._moveInput.y = Input.GetAxis("Vertical");
        pc._moveInput.Normalize(); 

        Rigidbody _rb = pc.GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(pc._moveInput.x * pc.getMoveSpeed(), _rb.velocity.y, pc._moveInput.y * pc.getMoveSpeed());

        pc.spriteAnimator.SetFloat("moveSpeed", _rb.velocity.magnitude);

        RaycastHit hit;

        if (Physics.Raycast(pc.groundPoint.position, Vector3.down, out hit, .3f, pc.groundLayer))
        {
            pc._isGrounded = true;
        }
        else pc._isGrounded = false;

        pc.spriteAnimator.SetBool("onGround", pc._isGrounded);

        if (!pc._isFlipped && pc._moveInput.x < 0)
        {
            pc._isFlipped = true;
            //_flipX = true; // todo: this should be made observable somehow
            //_spriteRenderer.flipX = _flipX;
            pc.spriteAnimator.SetBool("swapX", true);
            pc._spriteRenderer.flipX = pc.spriteAnimator.GetBool("swapX");
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        } else if (pc._isFlipped && pc._moveInput.x > 0)
        {
            pc._isFlipped = false;
            //_flipX = false;
            //_spriteRenderer.flipX = _flipX;
            pc.spriteAnimator.SetBool("swapX", false);
            pc._spriteRenderer.flipX = pc.spriteAnimator.GetBool("swapX");
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        }

        if (!pc._isBackwards && pc._moveInput.y > 0)
        {
            pc._isBackwards = true;
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        } else if (pc._isBackwards && pc._moveInput.y < 0)
        {
            pc._isBackwards = false;
            if(pc.flipEnabled) pc._playerOrientation.SetTrigger("Flip");
        }

        pc.spriteAnimator.SetBool("movingBackwards", pc._isBackwards);
    }


    public virtual void OnEnter(BR_PlayerController br_pc) {
        this.pc = br_pc;
    }
    public virtual void OnExit() {}
}
