using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BaseballManager : MonoBehaviourPun, IPunObservable
{
    private int _baseballLevel;
    
    
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
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(_baseballLevel);
        }
        else
        {
            // Network player, receive data
            this._baseballLevel = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void IncreaseBaseball()
    {
        if (PhotonNetwork.IsMasterClient) _baseballLevel++;
    }
    
    
    
    
}
