using UnityEngine;
// any possible event that could count as baseball
public class BaseballEvent
{
    public enum BaseballEventType
    {
        Hit, Run
    }

    public Vector3 EventPos;
    public BaseballEventType Event;
    public int BaseballEventMagnitude;

    public BaseballEvent(BaseballEventType eventType, Vector3 eventPos)
    {
        Event = eventType;
        EventPos = eventPos;
    }
}