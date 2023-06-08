using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
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
    [Header("Temp")]
    public GameObject[] temporaryBases;
    [Header("Debug")]
    [SerializeField] private Color infieldColor, outfieldColor;
    
    [Header("Settings")]
    [Tooltip("In seconds")][SerializeField] private int meterDrainFrequency = 5;
    [SerializeField] private int meterDrainAmount = 2;
    [SerializeField] private bool isMeterRunning;
    [SerializeField] private Vector2Int meterDefaultMax;

    public Vector2Int MeterDefaultMax => meterDefaultMax;

    private float _meterTimeRemaining;
    private int _baseballLevel;
    private int _currentLevel;
    public int BaseballLevel
    {
        get => _currentLevel;

        private set
        {
            if (value != _currentLevel)
            {
                _currentLevel = value;
                UpdateMeterUI();
            }
        }
    }

    private void UpdateMeterUI()
    {
        UpdateBballMeter?.Invoke(BaseballLevel); 
    }

    private BaseballCondition _currentBaseballCondition;
    private Dictionary<BaseballAction, float> _eventLog = new Dictionary<BaseballAction, float>();

    public static event Action<int> UpdateBballMeter;
    
    private void Start()
    {
        isMeterRunning = true;
        BaseballLevel = meterDefaultMax.x;
        /* temp stuff: eventually the blueprints will handle base setup */
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
        if (stream.IsWriting) // todo: can this be optimized to send more piecemeal data if necessary?
        {
            // We own this player: send the others our data
            stream.SendNext(_currentBaseballCondition); // does this receive a reference or the actual object??
            stream.SendNext(_eventLog);
            stream.SendNext(BaseballLevel);
        }
        else
        {
            // Network player, receive data
            this._currentBaseballCondition = (BaseballCondition)stream.ReceiveNext();
            this._eventLog = (Dictionary<BaseballAction, float>)stream.ReceiveNext();
            this.BaseballLevel = (int)stream.ReceiveNext();
        }
    }
    #endregion
    
    
    
    private void RegisterBaseballEvent (BaseballAction baseballAction)
    {
        /*
         * 
         */
        
        
        if (!PhotonNetwork.IsMasterClient) return;
        
        
        // this can probably be handled elsewhere and once?
        if (_currentBaseballCondition == null) _currentBaseballCondition = new BaseballCondition();

        if (baseballAction.Event == BaseballAction.BballActionType.Ignore)
        {
            Debug.Log("Baseball Event Ignored");
            return;
        }
        
        _eventLog.Add(baseballAction, Time.time); // log & eventually display this at least for debug purposes

        
        
        if (baseballAction.Event == BaseballAction.BballActionType.Result)
        {
            GradeBballAction(baseballAction);
            return;
        }
        
        
        RegisterBaseballEvent(_currentBaseballCondition.ValidateBballEvent(baseballAction));
        // if false: grade the event 
        
    }

    private void BaseballMeterTimer()
    {
        //if (isMeterDrainActive)
        // todo: hmm need to track the 'baseball value' here

        if (_meterTimeRemaining > 0) _meterTimeRemaining -= Time.deltaTime;

        else
        {
            if (_currentLevel <= 0)
            {
                _meterTimeRemaining = 0;
                isMeterRunning = false;
            }
            else
            {
                BaseballLevel -= meterDrainAmount; // lower the meter
                _meterTimeRemaining = meterDrainFrequency;
            }
        }
    }
    
    
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; // timer runs on on master client only
        
        if (isMeterRunning) BaseballMeterTimer();
    }

    private void GradeBballAction(BaseballAction baseballAction)
    {
        switch (baseballAction.Result)
        {
            case BaseballAction.BballResultType.None:
                break;
            case BaseballAction.BballResultType.Hit:
                BaseballLevel += (int)baseballAction.Magnitude;
                break;
            case BaseballAction.BballResultType.BaseRun:
                BaseballLevel += (int)baseballAction.Magnitude;
                break;
        }
        
        print(baseballAction);
        
        // todo: 'score' the baseball-ey-ness
        // todo: fire off resulting events (increment the meter, etc) 
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
        Gizmos.DrawCube(_currentBaseballCondition.WholeField.center, _currentBaseballCondition.WholeField.size);
        
    }

    private void OnEnable()
    {
        //todo: migrate to entity system
        GenericBall.BallgameEvent += RegisterBaseballEvent;
        BR_BballBase.BallgameEvent += RegisterBaseballEvent;
        UpdateBballMeter?.Invoke(BaseballLevel);
    }

    private void OnDisable()
    {
        GenericBall.BallgameEvent -= RegisterBaseballEvent;
        BR_BballBase.BallgameEvent -= RegisterBaseballEvent;
    }
}
