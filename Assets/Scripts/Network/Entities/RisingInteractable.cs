using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public interface RisingInteractable
{
    public void Interact();
    [PunRPC] public void Hit(Vector3 pos, float power);
    public void Throw(Vector3 direction, Collider playerCollider);
    public void ItemDestroyed();
    
}


