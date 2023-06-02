using System.Collections.Generic;
using UnityEngine;

// class to store the current condition of the baseball game i.e.: field, score, props, etc.
// 
public class BaseballCondition
{
    public enum BaseballState
    {
        Active, Inactive
    }

    public enum Bases
    {
        Home, First, Second, Third, Pitchers
    }
    
    private GameObject _first, _second, _third, _home, _pitchers;

    private Vector3[] _baseballDiamond;

    private BaseballState _currentBaseballState; 
    public Bounds Infield { get; private set; } 
    public Bounds Outfield { get; private set; }
    // public Bounds Homerun { get; }
    // public Bounds Foul { get; }

    // default constructor creates an inactive baseball game with no bases, etc
    public BaseballCondition(BaseballState state = BaseballState.Inactive)
    {
        _currentBaseballState = state;
        _baseballDiamond = new Vector3[4];
    }
    
    // todo: check that ball(s) exist?; bat(s)?

    public void AddBase(GameObject go, Bases position)
    {
        switch (position)
        {
            case Bases.Home:
                _home = go;
                break;
            case Bases.First:
                _first = go;
                break;
            case Bases.Second:
                _second = go;
                break;
            case Bases.Third:
                _third = go;
                break;
            case Bases.Pitchers:
                _pitchers = go;
                break;
        }

        // confusing language but, put the base in an array in the correct order
        // 0 = home, 1 = first, 2 = second, etc.
        _baseballDiamond[(int)position] = go.transform.position;

        if (_baseballDiamond.Length < 3) return;
        
        CalculateInfield();
        CalculateFieldBounds();

    }
    
    public void CalculateFieldBounds()
    {
        var homeFirst = Vector3.Distance(_baseballDiamond[0], _baseballDiamond[1]); // home-first
        
        Vector3[] outfield = 
        {
            _baseballDiamond[0],
            _baseballDiamond[0] + Vector3.forward * (homeFirst *2),
            _baseballDiamond[0] + Vector3.left * (homeFirst *2)
        };
        
        Outfield = GeometryUtility.CalculateBounds(outfield, Matrix4x4.identity);
    }
    
    public void CalculateInfield()
    {
        Infield = GeometryUtility.CalculateBounds(_baseballDiamond, Matrix4x4.identity);
    }

    public bool CheckIntersection(Bounds bound, Vector3 point)
    {
        return bound.Contains(point);
    }

    public BaseballStatus ValidateBballEvent(BaseballEvent baseballEvent)
    {
        switch (baseballEvent.Event)
        {
            case BaseballEvent.BaseballEventType.Hit:
                if (CheckIntersection(Infield, baseballEvent.EventPos)) return BaseballStatus.Minor;
                break;
            
            default: 
                return BaseballStatus.None;
        }

        return BaseballStatus.None;
    }
}