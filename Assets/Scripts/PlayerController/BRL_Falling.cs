using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Falling : BRL_BaseState
{
    public BRL_Falling() {
        stateName = BR_PlayerController.PlayerState.Falling;
    }
    
    public override void OnUpdate() {
        base.OnUpdate();
        // todo
        if (pc._isGrounded) {
            if (Mathf.Abs(pc._moveInput.x) > Mathf.Epsilon || Mathf.Abs(pc._moveInput.y) > Mathf.Epsilon) {
                pc.changeState(pc.Running);
            } else {
                pc.changeState(pc.Idle);
            }
        }
    }
}
