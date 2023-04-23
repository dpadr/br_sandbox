using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boar : MonoBehaviourPun, RisingEnemy
{
    private bool _isLive;
    private int _hitPoints;
    //private WaitForSeconds _wait;
    
    [Header("Parameters")]
    [SerializeField] private float timeTillLive = 2;
    [SerializeField] private int hitPoints;
    //[SerializeField] private int angryThreshold;

    [Header("Walk Settings")]
    [SerializeField] private float timeInterval;
    [SerializeField] private float speed;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool constantStep;
    
    private Vector2 _dir2D;
    private Vector3 _dir3D;

    private void Start()
    {
        _hitPoints = hitPoints;
        StartCoroutine(Walk());
     
    } 

    void RisingEnemy.Attack()
    {
        
    }

    public int Damage { get; set; }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_isLive) return;
        
        if (!other.CompareTag("Player")) return;
        
        if (other.TryGetComponent(out PlayerInteractable playerInteractable))
        {
            playerInteractable.DamagePlayer();
        }
    }

    IEnumerator Walk()
    {
        if (isWalking)
        {
            yield return new WaitForSeconds(timeInterval);
            if (constantStep)
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
        if (isWalking)
            transform.position = Vector3.MoveTowards(
                transform.position,
                transform.position + _dir3D,
                Time.deltaTime * speed
            );
    }
}
