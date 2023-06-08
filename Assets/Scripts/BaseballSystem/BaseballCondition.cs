using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* class to store the current condition of the baseball game i.e.: field, score, props, etc.
 * todo: 
 * this class contains a bunch of validation methods but im assuming for network performance
 * that it doesn't matter how much logic it contains vs how much data
*/

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
    
    public bool PlayOngoing { get; private set; }

    public string ActiveHitter { get; private set; }
    
    private GameObject _first, _second, _third, _home, _pitchers;
    private GameObject[] _baseballDiamondGOs;
    
    private Vector3[] _baseballDiamondPos;

    private BaseballState _currentBaseballState; 
    public Bounds Infield { get; private set; } 
    public Bounds WholeField { get; private set; }

    private Bounds _height;
    // public Bounds Homerun { get; }
    // public Bounds Foul { get; }

    private string _lastHitter; //todo: eventually player type

    // default constructor creates an inactive baseball game with no bases, etc
    public BaseballCondition(BaseballState state = BaseballState.Inactive)
    {
        _currentBaseballState = state;
        _baseballDiamondPos = new Vector3[4];
        _baseballDiamondGOs = new GameObject[4];
    }
    
    // todo: check that ball(s) exist?; bat(s)?

    public void AddBase(GameObject go, Bases position)
    {
        switch (position)
        {
            case Bases.Home:
                _home = go;
                CalculateHeight();
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
        
        //todo: can probably organize bases/diamond with pointers
        
        _baseballDiamondPos[(int)position] = go.transform.position;
        _baseballDiamondGOs[(int)position] = go;
        if (_baseballDiamondPos.Length < 3) return;
        
        // todo: these steps can probably all be collapsed into one method
        
        CalculateInfield();
        CalculateField();

    }

    private void CalculateHeight()
    {
        /* take one base (home) and use that to set a height min/max for the field bounds */
        
        var floor = _baseballDiamondPos[0];
        var ceiling = floor;
        floor.y = 0;
        ceiling.y = 1;
        var tempBounds = new Bounds();
        tempBounds.Encapsulate(floor);
        tempBounds.Encapsulate(ceiling);
        
        _height = tempBounds;
    }

    private void CalculateField()
    {
        /* this version is less dynamic for now, reducing the ability for players to squash/stretch the field */
        
        var homeFirst = Vector3.Distance(_baseballDiamondPos[0], _baseballDiamondPos[1]); // home-first

        Bounds tempBounds = new Bounds(); // some BS about structs hence cant access bounds normally (directly)
        
        Vector3[] outfield = 
        {
            _baseballDiamondPos[0],
            _baseballDiamondPos[0] + Vector3.forward * (homeFirst *2),
            _baseballDiamondPos[0] + Vector3.left * (homeFirst *2),
            
        };
        
        tempBounds = GeometryUtility.CalculateBounds(outfield, Matrix4x4.identity);
        tempBounds.Encapsulate(_height); // add the height
        
        WholeField = tempBounds;

    }

    private void CalculateInfield()
    {
        Bounds tempBounds = new Bounds();
        tempBounds = GeometryUtility.CalculateBounds(_baseballDiamondPos, Matrix4x4.identity);
        tempBounds.Encapsulate(_height);

        Infield = tempBounds;
    }

    private bool CheckIntersection(Bounds bound, Vector3 point)
    {
        /* todo: maybe this navigates a hierarchy of bounds (infield, outfield, etc)*/
        
        return bound.Contains(point);
    }

    public BaseballAction ValidateBballEvent(BaseballAction baseballAction)
    {
        /*
         * Probably need a scriptable object or some such way of storing the mapping of all the various results
         * so they can easily be modified
         */
        switch (baseballAction.Event)
        {
            case BaseballAction.BballActionType.Hit:
                return CheckHit(baseballAction);
            case BaseballAction.BballActionType.HitLand:
                return CheckHitLand(baseballAction);
            case BaseballAction.BballActionType.TagBase:
                return CheckBaseRun(baseballAction);
            default:
                return new BaseballAction(); // returns an Ignore event
        }
    }

    private BaseballAction CheckBaseRun(BaseballAction baseballAction)
    {
        if (!PlayOngoing) return new BaseballAction();
        
        // or don't ?? for the funnies

        if (_baseballDiamondGOs.Contains(baseballAction.EventOriginObject)) // is there a valid base
        {
            /*
             * todo: depending on which base was tagged ... something
             * todo: going to need more info like player and base info perhaps
             */
            
            return new BaseballAction(BaseballAction.BballResultType.BaseRun, BaseballAction.BballEventMagnitude.Low);
        }
        
        
        /*
         * interestingly, maybe one of the ways that the game can be wackier is by bending the validatation rules
         * on occasion or in particular circumstances
         */


        return new BaseballAction();
    }

    private BaseballAction CheckHitLand(BaseballAction hitLand)
    {
        /*
         * todo: what if the ball is caught or hits a player, object or NPC
         */
        
        // todo: add home run check
        
        if (CheckIntersection(Infield, hitLand.EventPos))
            return new BaseballAction(BaseballAction.BballResultType.BaseHit, BaseballAction.BballEventMagnitude.Low);
        if (CheckIntersection(WholeField, hitLand.EventPos))
            return new BaseballAction(BaseballAction.BballResultType.DriveHit, BaseballAction.BballEventMagnitude.Med);
        return new BaseballAction(BaseballAction.BballResultType.Foul, BaseballAction.BballEventMagnitude.Low);
    }

    private BaseballAction CheckHit(BaseballAction hit)
    {
        // todo: later i guess this checks only if the ball was hit near home plate?

        if (CheckIntersection(Infield, hit.EventPos))
        {
            PlayOngoing = true; 
            return new BaseballAction(BaseballAction.BballResultType.Hit, BaseballAction.BballEventMagnitude.Low);
        }

        return new BaseballAction(); // returns an Ignore event
    }
}