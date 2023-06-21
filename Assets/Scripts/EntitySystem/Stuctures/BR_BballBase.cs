using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class BR_BballBase : BR_Structure
{

    public static event Action<BaseballAction> BallgameEvent;
    
    internal enum BaseTagType {
        Enter, Leave 
    }
    
    // todo: should maybe be moved to triggers?
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandleTagBase(BaseTagType.Enter);
            
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) HandleTagBase(BaseTagType.Leave);
    }

    private void HandleTagBase(BaseTagType type)
    {
        switch (type)
        {
            case BaseTagType.Enter:
                var x = new BaseballAction(BaseballAction.BballActionType.TagBase, transform.position, this.gameObject,
                    "player");
                CallBaseballEvent(x);
                break;
            case  BaseTagType.Leave:
                var y = new BaseballAction(BaseballAction.BballActionType.LeaveBase, transform.position,
                    this.gameObject, "player");
                CallBaseballEvent(y);
                break;
        }
    }

    private static void CallBaseballEvent(BaseballAction action)
    {
        BallgameEvent?.Invoke(action); // todo: possible null ref?
    }
}