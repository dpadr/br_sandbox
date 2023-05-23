using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface IF_Carryable
{
    [PunRPC] public void PickUp(BR_Creature carrier);
    [PunRPC] public void PutDown();
    [PunRPC] public bool Held();
}
