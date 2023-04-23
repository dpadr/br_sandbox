using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EggBall : GenericBall
{
    [Header("Parameters")] [SerializeField]
    private int hitPoints; 
    
    // Start is called before the first frame update
    void Start() => _hitPoints = hitPoints;

// todo: some kind of inheritance issue but this aint working
    private new void ItemDestroyed()
    {
        print(name + " was destroyed !");
        PhotonNetwork.InstantiateRoomObject("egg_broken", transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
       // if (collision.gameObject.CompareTag("Ground")) _hitPoints--;
    }
}
