using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class Bee : MonoBehaviourPun, RisingEnemy
{
    private bool _isLive;
    //private WaitForSeconds _wait;
    
    [Header("Parameters")]
    [SerializeField] private float timeTillLive = 2;

    [Header("Walk Settings")]
    [SerializeField] private float _timeInterval;
    [SerializeField] private float _speed;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _constantStep;
    
    private Vector2 _dir2D;
    private Vector3 _dir3D;

    private void Start()
    {

        StartCoroutine(Walk());
        StartCoroutine(MakeLive());  
    } 

    void RisingEnemy.Attack()
    {
        
    }

    public int Damage { get; set; }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isLive) return;
        
        if (!other.CompareTag("Player")) return;
        
        if (other.TryGetComponent(out BR_PlayerController playerController))
        {
            playerController.photonView.RPC("DamagePlayer", RpcTarget.AllBuffered);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    
    // just adds a little delay so the bee isn't deadly on spawning
    IEnumerator MakeLive()
    {
        yield return new WaitForSeconds(timeTillLive);

        _isLive = true;
    }
    
    IEnumerator Walk()
    {
        
        if (_isWalking)
        {
            yield return new WaitForSeconds(_timeInterval);
            if (_constantStep)
            {
                float theta = Random.Range(0f, 2 * Mathf.PI);
                _dir2D = Vector2.right * Mathf.Cos(theta) 
                         + Vector2.up * Mathf.Sin(theta);
            }
            else
                _dir2D = Random.insideUnitCircle;

            _dir3D = Vector3.right * _dir2D.x + Vector3.forward * _dir2D.y;
        }

        
        StartCoroutine(Walk());
    }
    
    void Update()
    {
        if (_isWalking)
            transform.position = Vector3.MoveTowards(
                transform.position, 
                transform.position + _dir3D, 
                Time.deltaTime * _speed
            );
    }
    
    
    
}
