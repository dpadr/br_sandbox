using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Jumping : BRL_BaseState
{
    // private Vector2 _moveInput;
    public BRL_Jumping() {
        stateName = BR_PlayerController.PlayerState.Jumping;
    }

    public override void OnUpdate() {
        base.OnUpdate();
        Rigidbody _rb = pc.GetComponent<Rigidbody>();
        _rb.velocity += new Vector3(0f, pc.getJumpForce(), 0f);

        if (pc._isGrounded) {
            if (Mathf.Abs(pc._moveInput.x) > Mathf.Epsilon || Mathf.Abs(pc._moveInput.y) > Mathf.Epsilon) {
                pc.changeState(pc.Running);
            } else {
                pc.changeState(pc.Idle);
            }
        }
    }
}
