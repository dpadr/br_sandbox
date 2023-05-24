using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// these classes stored here only temporarily 

public class BaseballManager : MonoBehaviourPun, IPunObservable
{
    private int _baseballLevel;
    private BaseballCondition _currentBaseballCondition;
    public GameObject[] temporaryBases;
    
 /*
  * OK, SO:
  * Entities doing baseball events (aka: doing baseball things) on local client will
  * get validated by this script.  If they are valid, they will write directly to the network
  * 'baseball value'. This way any client who does a 'baseball thing' will be reporting.
  * The baseball value can then be read directly by the UI on each client 
  *
  * This implies that each clients baseball manager will need to be up to date with the
  * the current state of 'baseball' in order to do the verification process.
  * todo: this should probably be its own data structure
  *
  * Lastly, the incoming 'baseball events' should be their own type so that we can
  * differentiate and perhaps grade the types of events for things like achievements but
  * mainly to provide appropriate feedback to the player.
  * 
  */ 
    
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

    private void Update()
    {
        UpdateBaseballState();
    }

    private void UpdateBaseballState()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (_currentBaseballCondition == null) InitBaseballState();
        
        
    }

    private void InitBaseballState()
    {
        // (temp) manually set bases
        _currentBaseballCondition = new BaseballCondition(temporaryBases[0], BaseballCondition.BasePosition.Home);
    }

    public void RegisterBaseballEvent (BaseballEvent baseballEvent) {
        
        // if true: grade the event and propagate changes
        
        // if false: grade the event 
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
