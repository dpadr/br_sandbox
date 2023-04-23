using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ReviveBuoy : MonoBehaviourPun
{
    [SerializeField] private PlayerController playerController;

    public void BuoyRevive()
    {
        //playerController.RevivePlayer();
        playerController.photonView.RPC("RevivePlayer", RpcTarget.AllBuffered);
    }
    
}
