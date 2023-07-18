using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BR_Boar : BR_Animal, IInRoomCallbacks
{
    [SerializeField]
    private int maxHealth = 10;

    private BoarBT _boarTree;

    private void OnEnable()
    {
        // PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        // PhotonNetwork.RemoveCallbackTarget(this);
    }

    protected void Start()
    {
        base.Start(); //calls the start function of the parent class (in this case, bc Animal has no start, it calls Creature's)
        _healthComponent.SetMaxHealth(maxHealth);
        //boarBT = new BoarBT(transform);
        _boarTree = gameObject.AddComponent<BoarBT>();
        _boarTree.enabled = PhotonNetwork.IsMasterClient;

    }
    protected override void OnUpdate()
    {

        /* todo: this is a terrible hack but i can't seem to get the IInroomcallbacks to work */
        _boarTree.enabled = PhotonNetwork.IsMasterClient;

    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        _healthComponent = GetComponent<BR_HealthComponent>();
        _healthComponent.SetMaxHealth(maxHealth);
#endif
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
       
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
       
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
       
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
       
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        print("master client switched");
        _boarTree.enabled = PhotonNetwork.IsMasterClient;
    }
}