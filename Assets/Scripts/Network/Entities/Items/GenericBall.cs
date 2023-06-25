using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using VectorForestScenery;

[RequireComponent(typeof(Collider))]
public class GenericBall : RisingPickup, RisingInteractable
{
    private bool _canBeHit = true;
    private bool _ballIsLive, _ballIsAirborne;
    
    protected int _hitPoints = -1;
    
    public static Action<BaseballAction> BallgameEvent;

    [SerializeField] private float triggerActiveDelay = .1f;
    [SerializeField] private float forceMultiplier = 50;
    [SerializeField] private float airMultiplier = 50;
    [SerializeField] private float groundBallActive = 1.5f;
    public void Interact() {}

    private static void CallBaseballEvent(BaseballAction theAction)
    {
        BallgameEvent?.Invoke(theAction);
    }

    [PunRPC]
    public void Hit(Vector3 pos, float power)
    {
        if (!_canBeHit) return;
        if (_isHeld) return;
        
        _rigidbody.isKinematic = false;
        _rigidbody.AddForceAtPosition(Vector3.forward * power + Vector3.up * power +  Vector3.left * power, pos, ForceMode.Impulse);
        print(name + " was hit !");

        _ballIsLive = true; // not sure what this does yet
        _ballIsAirborne = true;

        var newEvent = new BaseballAction(BaseballAction.BballActionType.Hit, transform.position, this.gameObject, "temp");
        CallBaseballEvent(newEvent);
        
        /* not sure what this is about any more */
        if (_hitPoints == -1) return;
        if (_hitPoints == 0) ItemDestroyed();
        if (_hitPoints > 0)
        {
            print(name + " hp " + _hitPoints);
            _hitPoints--;
        }


    }


//todo: not working for master client?!

    public void Throw(Vector3 direction, Collider playerCollider)
    {
        Drop();
        Physics.IgnoreCollision(_collider, playerCollider, true);
        _canBeHit = true;
        _rigidbody.isKinematic = false;
    _rigidbody.AddRelativeForce(direction * forceMultiplier + (Vector3.up * airMultiplier));
    StartCoroutine(DelayTriggerEnable());
// todo: re-enable collisions
    }

    public void ItemDestroyed()
    {
        
    }

    public void Damage()
    {
        
    }


    /* check whether the ball can be hit -- disabled this stuff for now */
    
    IEnumerator DelayTriggerEnable()
    {
        yield return new WaitForSeconds(triggerActiveDelay);
        _collider.isTrigger = false;    
    }

    IEnumerator DelayBallCanBeHit()
    {
        yield return new WaitForSeconds(groundBallActive);
        _canBeHit = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (_ballIsLive && _ballIsAirborne) CallBaseballEvent(new BaseballAction(BaseballAction.BballActionType.HitLand, 
                transform.position, this.gameObject, "temp"));

            _ballIsAirborne = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Folliage"))
        {
            if (other.TryGetComponent(out SceneryItem sceneryItem))
            {
                sceneryItem.TriggerParticle();
            }
        }

        if (other.CompareTag("Player") && _ballIsAirborne)
        {
            // todo: knockout the player?
        }
    }

}
