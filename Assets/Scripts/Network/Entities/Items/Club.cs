using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Club : RisingPickup, RisingInteractable
{
    [Header("Setup")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask interactable;

    [Header("Parameters")] 
    [SerializeField] private Vector2 hitPower;
    [SerializeField] private float swingRadius = 2.5f; 
    
    [PunRPC]
    public override void Drop()
    {
        if (_collider != null) _collider.isTrigger = false;
        _isHeld = false;
    }

    public void Interact()
    {
        print("swing batta bata");
        animator.SetTrigger("swing");
        audioSource.Play();
        foreach (var x in GetClosestItem())
        {
            if (x.TryGetComponent(out GenericBall ball))
            {
                //ball.photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                ball.photonView.RPC("Hit", RpcTarget.AllBuffered, transform.position, Random.Range(hitPower.x, hitPower.y));
                //ball.Hit(transform.position, Random.Range(hitPower.x, hitPower.y));
                
            } else if (x.TryGetComponent(out RisingInteractable interact))
            {
                interact.Hit(transform.position, hitPower.x);
            }
        }
    
    }

    public void Hit(Vector3 pos, float power)
    {
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


    private List<GameObject> GetClosestItem()
    {
        /* Pickup the closest item */
        int maxColliders = 5;
        Collider[] hitColliders = new Collider[maxColliders];
        if (Physics.OverlapSphereNonAlloc(transform.position, swingRadius, hitColliders, interactable,
                QueryTriggerInteraction.Collide) > 0)
        {
            // todo: this can be cleaned up no doubt
            var temp = hitColliders.ToList();
            // remove anything not interactable or null
            temp.RemoveAll(x => x == null);
            

            List<GameObject> gameObjects = new List<GameObject>();
            
            foreach (var x in temp)
            {
                gameObjects.Add(x.gameObject);
            }
            
            
            if (gameObjects.Count > 0)
            {
                // otherwise return the closest interactable
                return gameObjects;
            }
        }
        print("no item found");
        return null;
    }
}
