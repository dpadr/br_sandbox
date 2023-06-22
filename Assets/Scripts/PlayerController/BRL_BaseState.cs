using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/*  Base State for BR_PlayerController State Machine. */
public class BRL_BaseState
{
    // reference to BR_PlayerController
    protected BR_PlayerController pc; 

    // state name enum for displaying current state in BR_PlayerController
    public BR_PlayerController.PlayerState stateName;

    // gets run in Update; will be built upon by other states
    //      applies moving speed, does grounded check, flips sprites
    //      allows for Pick Up, Drop, Pitch, Interact
    public virtual void OnUpdate() {

        if (pc._isDead) {
            pc.changeState(pc.Dead);
            return;
        }

        pc._moveInput = pc.movement.ReadValue<Vector2>();
        pc._moveInput.Normalize(); 

        Rigidbody _rb = pc.GetComponent<Rigidbody>();
        _rb.velocity = new Vector3(pc._moveInput.x * pc.getMoveSpeed(), _rb.velocity.y, pc._moveInput.y * pc.getMoveSpeed());

        pc.spriteAnimator.SetFloat("moveSpeed", _rb.velocity.magnitude);

        RaycastHit hit;

        if (Physics.Raycast(pc.groundPoint.position, Vector3.down, out hit, .3f, pc.groundLayer))
        {
            pc._isGrounded = true;
        }
        else 
        {
            pc._isGrounded = false;
            pc.changeState(pc.Falling);
        }

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
        
        // pick up items; cannot pick up during jumping
        if (pc.pickup.triggered && pc._isGrounded) {
            var pickup = pc.PickupItem();
            if (pickup == null) return;
            
            if (pickup.TryGetComponent(out ReviveBuoy reviveBuoy))
            {
                reviveBuoy.BuoyRevive();
                return;
            }

            if (pickup.TryGetComponent(out RisingPickup risingPickup))
            {
                if (risingPickup.CheckIfHeld()) return;
            }

            /*if(pickup.TryGetComponent(out IF_Carryable carryable))
            {
                if (carryable.Held()) return;
                carryable.PickUp(this);
            }*/
            

            if (pc._heldItem != null) pc.DropItem();
            
            pc._heldItem = pickup;
            var view = pc._heldItem.GetPhotonView();
            if (view != null) view.TransferOwnership(PhotonNetwork.LocalPlayer);
            pc.AttachItem(pc._heldItem);
        }

        // drop item
        if (pc.drop.triggered) {
            pc.DropItem();
        }

        // use item / interact with item
        if (pc.interact.triggered) {
            pc.UseItem();
        }

        // throw item
        if (pc.pitch.triggered) {
            // todo: allow the player to charge the throw
            // probably needs to add a Hold Interaction for the throw action
            if (pc.ThrowItem(ref pc._heldItem, _rb.velocity)) pc.audioSource.Play();
        }
    }


    public virtual void OnEnter(BR_PlayerController br_pc) {
        this.pc = br_pc;
    }
    public virtual void OnExit() {}
}
