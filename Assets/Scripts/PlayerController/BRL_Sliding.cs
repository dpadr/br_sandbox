using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Sliding : BRL_BaseState
{
    public BRL_Sliding() {
        stateName = BR_PlayerController.PlayerState.Sliding;
    }
    
    public override void OnUpdate() {
        base.OnUpdate();
        // todo
    }
}
