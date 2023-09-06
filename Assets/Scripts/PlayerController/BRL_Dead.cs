using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Dead : BRL_BaseState
{
    public BRL_Dead() {
        stateName = BR_PlayerController.PlayerState.Dead;
    }
    
    public override void OnUpdate() {
        // does nothing, except changes back to idle when the player is not dead anymore
        if (!pc._isDead) {
            pc.changeState(pc.Idle);
        }
    }
}
