using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRL_Incapacitated : BRL_BaseState
{
    public BRL_Incapacitated() {
        stateName = BR_PlayerController.PlayerState.Incapacitated;
    }
    
    public override void OnUpdate() {
        base.OnUpdate();
        // todo
    }
}
