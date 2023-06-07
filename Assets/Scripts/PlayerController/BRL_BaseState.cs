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

    public string name;

    public virtual void OnUpdate() {}
    public virtual void OnEnter(BR_PlayerController br_pc) {
        this.pc = br_pc;
    }
    public virtual void OnExit() {}
}
