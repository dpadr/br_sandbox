using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using VectorForestScenery;
using Random = UnityEngine.Random;

public class GivingTree : MonoBehaviourPun, RisingInteractable, IPunObservable
{
    private SceneryItem _sceneryItem;
    
    [SerializeField] private string[] treeDropPrefabs;
    [SerializeField] private int itemDropQuantity, dropChance;
    [SerializeField] private GameObject treeSpawnLoc;
    private void Start()
    {
        if (TryGetComponent(out SceneryItem sceneryItem)) _sceneryItem = sceneryItem;
        
    }

    public void Interact()
    {
        if (_sceneryItem == null || treeDropPrefabs.Length < 1 || itemDropQuantity < 1) return;

        //photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        _sceneryItem.Shake();

        if (Random.Range(0, dropChance) % dropChance == 0)
        {
            itemDropQuantity--;
            DropItem(treeDropPrefabs[Random.Range(0, treeDropPrefabs.Length)]);
        }
        print("did interact");
    }

    private void DropItem(string prefabName)
    {
        PhotonNetwork.InstantiateRoomObject(prefabName, treeSpawnLoc.transform.position, Quaternion.identity);
    }
    
    public void Hit(Vector3 pos, float power)
    {
    
    }

    public void Throw(Vector3 direction, Collider playerCollider)
    {
    
    }

    public void ItemDestroyed()
    {
    
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
