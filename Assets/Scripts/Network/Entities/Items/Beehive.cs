using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Beehive : RisingPickup, RisingInteractable
{
    [SerializeField] private int maxBees = 5;

    private int _currentBees;
    // Start is called before the first frame update
    void Start()
    {
        _currentBees = maxBees;
    }
    
    public void Interact()
    {

        if (_currentBees < 1) return;
        
        _currentBees--;
        
        SpawnBee();
    }

    public void Hit(Vector3 pos, float power)
    {
        Hit();
    }

    private void SpawnBee(bool random = false)
    {
        if (!random)
        {
            var y = PhotonNetwork.Instantiate("Bee", transform.position, Quaternion.identity);
            
            return;
        }

        float x = Random.Range(-1.5f, 1.5f); float z = Random.Range(-1.5f, 1.5f);
        var randomPos = transform.position + new Vector3(x, 0, z);
        PhotonNetwork.Instantiate("Bee", randomPos, Quaternion.identity);
    }

    public void Hit()
    {
        print("hit" + name);
        for (var i = 0; i < _currentBees; i++)
        { 
            SpawnBee(true);    
        }
    }

    public void Throw(Vector3 direction, Collider playerCollider)
    {
        Drop();
    }

    public void ItemDestroyed()
    {
        
    }

    public void Damage()
    {
        
    }
}
