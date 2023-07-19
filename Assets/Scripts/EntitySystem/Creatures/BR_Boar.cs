using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BR_Boar : BR_Animal, IPunObservable
{
    [SerializeField]
    private int maxHealth = 10;

    private BehaviourTree _boarBehaviourTree; // Using : Tree -- eventually we can add this to field to Creature 
    
    private void OnEnable()
    {
        GameManager.MasterClientChange += MasterClientChange;
    }

    private void OnDisable()
    {
        GameManager.MasterClientChange -= MasterClientChange;
    }

    private void MasterClientChange()
    {
     // Debug.LogWarning("master client change");
     _boarBehaviourTree.enabled = PhotonNetwork.IsMasterClient;
     
     if (_boarBehaviourTree.enabled) Debug.Log(this + " enabled.");
     else Debug.Log(this + "disabled");
     
    }
    

    protected void Start()
    {
        base.Start(); //calls the start function of the parent class (in this case, bc Animal has no start, it calls Creature's)
        _healthComponent.SetMaxHealth(maxHealth);
        //boarBT = new BoarBT(transform);
        _boarBehaviourTree = gameObject.AddComponent<BoarBT>();
        _boarBehaviourTree.enabled = false;

    }
    protected override void OnUpdate()
    {
        StreamTest();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        _healthComponent = GetComponent<BR_HealthComponent>();
        _healthComponent.SetMaxHealth(maxHealth);
#endif
    }

    [ContextMenu("testing stream")]
    public void StreamTest()
    {
        maxHealth--;
    }
    
    /* Lore: apparently OnPhotonSerializeView is only called with 2 or more players in game */
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) 
        {
            // We own this player: send the others our data
            stream.SendNext(maxHealth); // does this receive a reference or the actual object??
            Debug.Log("write " + maxHealth);
        }
        else
        {
            // Network player, receive data
            this.maxHealth = (int)stream.ReceiveNext();
            Debug.Log("read " + maxHealth);
        }
        
    }
}