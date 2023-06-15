using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Running : BRL_BaseState
{
    public BRL_Running() {
        stateName = BR_PlayerController.PlayerState.Running;
    }
    // Start is called before the first frame update

    // public override void OnEnter(BR_PlayerController br_pc)
    // {
    //     base.OnEnter(br_pc);
    // }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Mathf.Abs(pc._moveInput.x) < Mathf.Epsilon || Mathf.Abs(pc._moveInput.y) < Mathf.Epsilon) {
            pc.changeState(pc.Idle);
        }

        if (pc.jump.action.triggered && pc._isGrounded)
        {
            //_rb.velocity += new Vector3(0f, jumpForce, 0f);
            // transition to Jump State
            pc.changeState(pc.Jumping);
        }
    }
}
