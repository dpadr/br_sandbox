using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/*
 * OK, SO:
 * Entities doing baseball events (aka: doing baseball things) on local client will
 * get validated via events here. If they are valid, this script will (also) call events to
 * update the state of various client-local objects (like the baseball meter)
 * This way any client who does a 'baseball thing' will be reporting to manager via RPC.
 * The baseball value can then be read directly by the UI on each client 
 *
 * This implies that each clients baseball manager will need to be up to date with the
 * the current state of 'baseball' in order to do the verification process. So we find out
 * who is the master client and only that client does the updates on the baseball state
 * but we still propagate state updates to all clients in case master client drops.
 *
 * Lastly, the incoming 'baseball events' should be their own type so that we can
 * differentiate and perhaps grade the types of events for things like achievements but
 * mainly to provide appropriate feedback to the player. (Same for blueprint events)
 */ 

public class BaseballManager : MonoBehaviourPun, IPunObservable
{
    public GameObject[] temporaryBases;

    [SerializeField] private Color infieldColor, outfieldColor;
    
    private int _baseballLevel;
    private BaseballCondition _currentBaseballCondition;
    private Dictionary<BaseballEvent, float> _eventLog = new Dictionary<BaseballEvent, float>();

    private void Start()
    {
        _currentBaseballCondition = new BaseballCondition();
        _currentBaseballCondition.AddBase(temporaryBases[0], BaseballCondition.Bases.Home);
        _currentBaseballCondition.AddBase(temporaryBases[1], BaseballCondition.Bases.First);
        _currentBaseballCondition.AddBase(temporaryBases[2], BaseballCondition.Bases.Second);
        _currentBaseballCondition.AddBase(temporaryBases[3], BaseballCondition.Bases.Third);
    }

    #region singleton

    public static BaseballManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    #endregion
    
    #region photon
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(_baseballLevel);
            stream.SendNext(_currentBaseballCondition);
            stream.SendNext(_eventLog);
        }
        else
        {
            // Network player, receive data
            this._baseballLevel = (int)stream.ReceiveNext();
            this._currentBaseballCondition = (BaseballCondition)stream.ReceiveNext();
            this._eventLog = (Dictionary<BaseballEvent, float>)stream.ReceiveNext();
            
            //todo: does it receive a reference or the actual object??
        }
    }
    #endregion
    [ContextMenu("RegisterEvent")]
    public void RegisterBaseballEvent (BaseballEvent baseballEvent)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (_currentBaseballCondition == null) _currentBaseballCondition = new BaseballCondition();
        
        // if true: grade the event and propagate changes

        print(baseballEvent);
        _eventLog.Add(baseballEvent, Time.time); 
        UpdateBballStatus(_currentBaseballCondition.ValidateBballEvent(baseballEvent));
        // if false: grade the event 
    }

    private void UpdateBballStatus(BaseballStatus status)
    {
        _baseballLevel += (int) status;
        print("baseball meter :" + _baseballLevel);
    }
    
    public bool RegisterBlueprintEvent()
    {
        if (!PhotonNetwork.IsMasterClient) return false; // ??
        
        if (_currentBaseballCondition == null) _currentBaseballCondition = new BaseballCondition();
        // todo: do blueprints have their own events?

        return true;
    }
    
    /*
     * todo: add the logics for 'scoring' the baseball events e.g.:
     * ball:
     * a ball is hit; a ball is hit within the batter box;
     * a ball strikes another player; a ball strikes the infield;
     * a ball strikes the outfiield; a ball strikes outside the field;
     * a ball strikes the home run zone; 
     */

    private void OnDrawGizmos()
    {
        // todo: draw the baseball field areas
        // todo: draw the baseball bases
        
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = infieldColor;
        Gizmos.DrawCube(_currentBaseballCondition.Infield.center, _currentBaseballCondition.Infield.size);
        Gizmos.color = outfieldColor;
        Gizmos.DrawCube(_currentBaseballCondition.Outfield.center, _currentBaseballCondition.Outfield.size);
    }

    private void OnEnable()
    {
        //todo: migrate to entity system
        GenericBall.BallgameEvent += RegisterBaseballEvent;
    }

    private void OnDisable()
    {
        GenericBall.BallgameEvent -= RegisterBaseballEvent;
    }
}

public enum BaseballStatus
{
    None, Minor, Medium, Major
}
