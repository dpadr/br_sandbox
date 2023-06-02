using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boar : MonoBehaviourPun, RisingEnemy
{
    private bool _isLive, _isBaseBlocking;
    private int _hitPoints;
    //private WaitForSeconds _wait;
    
    [Header("Parameters")]
    [SerializeField] private float timeTillLive = 2;
    [SerializeField] private int hitPoints;
    [SerializeField] private LayerMask interactable;
    //[SerializeField] private int angryThreshold;

    [Header("Walk Settings")]
    [SerializeField] private float timeInterval;
    [SerializeField] private float speed;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool constantStep;

    [Header("Baseball Interruption")] 
    [SerializeField] private int baseDetectRadius;
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

        var foundBase = LookForBase();

        if (foundBase == null)
        {
            StartCoroutine(Walk());    
        }
        else
        {
            _dir3D = foundBase.transform.position;
            isWalking = false;
            _isBaseBlocking = true;
        }
        
        
    }

    
    void Update()
    {
        if (isWalking)
            transform.position = Vector3.MoveTowards(
                transform.position,
                transform.position + _dir3D,
                Time.deltaTime * speed
            );
        
        if (_isBaseBlocking) 
            transform.position = Vector3.MoveTowards(
                transform.position, _dir3D + new Vector3(0, .3f, 0), Time.deltaTime * speed * 2);
    }

    private GameObject LookForBase()
    {
        int maxColliders = 5;
        Collider[] hitColliders = new Collider[maxColliders];
        if (Physics.OverlapSphereNonAlloc(transform.position, baseDetectRadius, hitColliders, interactable,
                QueryTriggerInteraction.Collide) > 0)
        {
            
            // todo: this can be cleaned up no doubt
            var temp = hitColliders.ToList();
            
            temp.RemoveAll(x => x == null);
            temp.RemoveAll(x => x.gameObject.TryGetComponent(out Baseball_Base bases) == false );
            // don't try to pickup the currently held item
            //if (_heldItem != null) temp.Remove(_heldItem.GetComponent<Collider>());
            var distanceSorted = temp.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).ToList();

            if (distanceSorted.Count > 0)
            {
                print("found " + distanceSorted[0].gameObject);
                return distanceSorted[0].gameObject;
            }
        }

        return null;
    }
}