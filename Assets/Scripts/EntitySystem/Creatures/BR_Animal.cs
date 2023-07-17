using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BR_Animal : BR_Creature
{
    AnimalState currentState = AnimalState.Idle;
    public enum AnimalState
    {
        Idle,
        Wandering,
        Agressive,
        Dead
    }
    

    public AnimalState GetState()
    {
        return currentState;
    }

    private void ChangeState(AnimalState targetState)
    {
        currentState = targetState;
    }
}
