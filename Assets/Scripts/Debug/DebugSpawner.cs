using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DebugSpawner : MonoBehaviourPun
{
    
    
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }
    
    public void Spawn()
    {
        PhotonNetwork.InstantiateRoomObject("skull_ball", transform.position, Quaternion.identity);
    }
}
