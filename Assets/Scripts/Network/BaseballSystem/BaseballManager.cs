using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
    private int _baseballLevel;
    private BaseballCondition _currentBaseballCondition;
    public GameObject[] temporaryBases;
    
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
        }
        else
        {
            // Network player, receive data
            this._baseballLevel = (int)stream.ReceiveNext();
            this._currentBaseballCondition = (BaseballCondition)stream.ReceiveNext();
        }
    }
    #endregion
    
    public void IncreaseBaseball()
    {
        if (PhotonNetwork.IsMasterClient) _baseballLevel++;
    }
    
    private void UpdateBaseballState()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (_currentBaseballCondition == null) InitBaseballState();
        
        
    }

    private void InitBaseballState()
    {
        // (temp) manually set bases
        _currentBaseballCondition = new BaseballCondition();
    }

    public void RegisterBaseballEvent (BaseballEvent baseballEvent) {
        
        // if true: grade the event and propagate changes
        
        // if false: grade the event 
    }
    
    

    public bool RegisterBlueprintEvent()
    {
        // todo: do blueprints have their own events?

        return true;
    }
    
    /*
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
        
    }
}
