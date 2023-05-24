using UnityEngine;

// class to store the current condition of the baseball game i.e.: field, score, props, etc.
public class BaseballCondition
{
    public enum BaseballState
    {
        Active, Inactive
    }

    public enum BasePosition
    {
        Home, First, Second, Third
    }
    public GameObject First, Second, Third, Home;
    // baseball field 'blueprints'
    private Vector3[] _baseballDiamond;

    private BaseballState _currentBaseballState; 
    public Bounds Infield { get; private set; }
    // public Bounds Outfield { get; }
    // public Bounds Homerun { get; }
    // public Bounds Foul { get; }

    public BaseballCondition(BaseballState state = BaseballState.Inactive)
    {
        //todo: have had weird experiences doing stuff within constructors but hey fuck it
    }

    // todo: ! this is gonna get weird because we get new each time there is a new base 
    public BaseballCondition(GameObject baseobject, BasePosition position) : this()
    {
        switch (position)
        {
            case BasePosition.Home:
                Home = baseobject;
                break;
            case BasePosition.First:
                First = baseobject;
                break;
            case BasePosition.Second:
                Second = baseobject;
                break;
            case BasePosition.Third:
                Third = baseobject;
                break;
        }
    }
    
    // todo: check that ball(s) exist?; bat(s)?

    public void CalculateFieldBounds()
    {
        // todo: eventually just do this once and optimize it
    }
    
    public void CalculateInfield()
    {
        // todo: generalize this for all field bounds
        _baseballDiamond = new Vector3[4];
        _baseballDiamond[0] = Home.transform.position;
        _baseballDiamond[1] = First.transform.position;
        _baseballDiamond[2] = Second.transform.position;
        _baseballDiamond[3] = Third.transform.position;

        // todo: very unsure about this one
        Infield = GeometryUtility.CalculateBounds(_baseballDiamond, Matrix4x4.identity);
    }

}