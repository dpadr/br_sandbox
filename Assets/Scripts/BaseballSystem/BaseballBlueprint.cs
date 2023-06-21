using System;
using Photon.Pun;
using UnityEngine;

public class BaseballBlueprint : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject bHome, bFirst, bSecond, bThird, bPitchers;

    private void OnEnable()
    {
        //sub
    }

    private void OnDisable()
    {
        //unsub
    }

    public enum BlueprintEvent
    {
        BaseAdded, BaseRemoved, BaseMoved, BallAdded, BatAdded
    } 
    
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
    
    
    
    // todo: tells the manager if baseball can be started
    
    //todo: maybe base bluerprints have cooldowns for when they can be empty
}