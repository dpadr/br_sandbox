using System;
using UnityEngine;
// any possible event that could count as baseball
public class BaseballAction
{
    public enum BballActionType
    {
        Ignore, Hit, HitLand, TagBase, LeaveBase, Catch, TagPlayer, Result
    }
    public enum BballEventMagnitude // not sure if this is needed
    {
        Low = 2, Med = 4, High = 6
    }
    
    public enum BballResultType
    {
        None, Hit, Foul, BaseHit, DriveHit, Out, BaseRun, HomeRun 
    }
    public readonly GameObject EventOriginObject;
    public readonly Vector3 EventPos;
    public readonly BballActionType Event;
    public readonly BballEventMagnitude Magnitude;
    public readonly BballResultType Result;
    public readonly string Player;

    /* default */
    public BaseballAction()
    {
        Event = BballActionType.Ignore;
    }

    /* for results */
    public BaseballAction(BballResultType resultType, BballEventMagnitude bballEventMagnitude)
    {
        Event = BballActionType.Result;
        Result = resultType;
        Magnitude = bballEventMagnitude;
    }

    /* for events */
    public BaseballAction(BballActionType eventType, Vector3 eventPos, GameObject go, string player)
    {
        Event = eventType;
        EventPos = eventPos;
        EventOriginObject = go;
        Player = player;
    }
    
    public override string ToString()
    {
        return Event switch
        {
            BballActionType.Ignore => "Baseball event ignored",
            BballActionType.Result => "Baseball Result: " + Result + "w/ Mag: " + Magnitude,
            _ => Player + "did Baseball Event:" + Event + " At:" + EventPos
        };
    }
}