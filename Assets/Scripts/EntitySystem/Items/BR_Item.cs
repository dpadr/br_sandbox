using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public abstract class BR_Item : BR_Entity, IF_Carryable
{
    private bool _carried;
    public bool Carried => _carried;

    private BR_Creature _carrier;
    public BR_Creature Carrier => _carrier;

    [PunRPC] public void PickUp(BR_Creature carrier)
    {
        _carried = true;
        _carrier = carrier;
        transform.position = _carrier.transform.position;
    }

    [PunRPC] public void PutDown()
    {
        _carried = false;
        _carrier = null;
    }

    [PunRPC] public bool Held()
    {
        return _carried;
    }
    protected override void OnUpdate()
    {
        if(Carried)
        {
            transform.position = _carrier.transform.position;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.Serialize(ref _carried);
        }
        else
        {
            stream.Serialize(ref _carried);
        }
    }
}
