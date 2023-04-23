using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RisingPickup : MonoBehaviourPun, IPunOwnershipCallbacks, IPunObservable
{
    protected bool _isHeld;
    private PhotonTransformView _transformView;
    private PhotonRigidbodyView _rigidbodyView;
    protected Collider _collider; 
    protected Rigidbody _rigidbody;

    public bool CheckIfHeld()
    {
        return _isHeld;
    }

    private void Awake()
    {
        if (TryGetComponent(out PhotonTransformView view)) _transformView = view;
        if (TryGetComponent(out PhotonRigidbodyView rbview)) _rigidbodyView = rbview;
        if (TryGetComponent(out Rigidbody rb)) _rigidbody = rb;
        if (TryGetComponent(out Collider col)) _collider = col;
    }
    
    
    [PunRPC]
    public virtual void Pickup()
    {
        _isHeld = true;
        if (_transformView != null) _transformView.enabled = true;
        if (_rigidbodyView != null) _rigidbodyView.enabled = false;
        if (_collider !=null) _collider.isTrigger = true;
        if (_rigidbody != null) _rigidbody.isKinematic = true;
    }

    // todo: convert to RPC? what settings need to change if any?
    [PunRPC]
    public virtual void Drop()
    {
        _isHeld = false;
        // photonView.TransferOwnership(PhotonNetwork.MasterClient);
        // //photonView.ControllerActorNr = 0;
        // if (_transformView != null) _transformView.m_UseLocal = false;
        // if (_collider !=null) _collider.isTrigger = false;
        // if (_rigidbody != null) _rigidbody.isKinematic = false;

    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (!targetView.IsMine) targetView.TransferOwnership(requestingPlayer);
        // todo: currently allows stealing of items

    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        print("ownership of: " + targetView + "transfered from: " + previousOwner);
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        print("couldnt transfer ownership of: " + targetView + " from: " + senderOfFailedRequest);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.Serialize(ref _isHeld);
        }
        else
        {
            stream.Serialize(ref _isHeld);
        }
    }

    
}
