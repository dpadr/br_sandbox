using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Idle : BRL_BaseState
{
    public BRL_Idle() {
        stateName = BR_PlayerController.PlayerState.Idle;
    }
    
    public override void OnUpdate() {
        base.OnUpdate();

        if (Input.GetButtonDown("Jump") && pc._isGrounded)
        {
            pc.changeState(pc.Jumping);
        }

        if (Mathf.Abs(pc._moveInput.x) > Mathf.Epsilon || Mathf.Abs(pc._moveInput.y) > Mathf.Epsilon) {
            pc.changeState(pc.Running);
        }
        
    }
}
