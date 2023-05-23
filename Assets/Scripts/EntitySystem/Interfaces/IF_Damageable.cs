using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public interface IF_Damageable
{
    //Takes an int for damage taken, and returns a bool which is true if the entity dies
    [PunRPC] public bool TakeDamage(int damage);
}
