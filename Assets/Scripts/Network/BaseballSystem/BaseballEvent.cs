using UnityEngine;
// any possible event that could count as baseball
public class BaseballEvent
{
    public enum BaseballEventType
    {
        Hit, HitLand, RunBase
    }

    public Vector3 EventPos;
    public BaseballEventType Event;
    public int BaseballEventMagnitude;
    public string Player;

    public BaseballEvent(BaseballEventType eventType, Vector3 eventPos)
    {
        Event = eventType;
        EventPos = eventPos;
    }

    public override string ToString()
    {
        return "Bball Event:" + Event + " At:" + EventPos;
    }
}