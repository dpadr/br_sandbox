using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public abstract class BR_Entity : MonoBehaviourPun, IPunOwnershipCallbacks
{
    private List<BR_Component> _components;
    protected abstract void OnUpdate();
    public enum EntityState
    {
        Active,
        Inactive,
        Destroy
    }

    private void Update()
    {
        if(_currentState == EntityState.Active)
        {
            OnUpdate();
            foreach (BR_Component component in _components)
            {
                component.OnUpdate();
            }
        }
    }

    private EntityState _currentState;
    public EntityState CurrentState => _currentState;

    [PunRPC]
    protected void SetState(EntityState state)
    {
        _currentState = state;
    }

    protected void AddComponent(BR_Component component, BR_Entity parent)
    {
        if (!_components.Contains(component))
        {
            _components.Add(component);
            component.Parent = parent;
        }
    }

    protected void RemoveComponent(BR_Component component)
    {
        component.Parent = null;
        _components.Remove(component);
    }

    private void OnDestroy()
    {
        BR_EntityManager.Instance.RemoveEntity(this);
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
}
