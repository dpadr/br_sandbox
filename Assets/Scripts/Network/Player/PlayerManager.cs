using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject localPlayerInstance;
    public float health = 3f;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(health);
        }
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
        }
    }
    private void Awake()
    {
        if (photonView.IsMine)
        {
            PlayerManager.localPlayerInstance = this.gameObject;
            
        }
        
        DontDestroyOnLoad(this.gameObject);
    }
    
}
