
using System.Linq;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, PlayerInteractable
{

    private Rigidbody _rb;
    private Collider _collider;
    private Vector2 _moveInput;
    private bool _isGrounded, _isFlipped, _isBackwards, _isCamActive, _flipX, _isDead;
    private Animator _playerOrientation;
    private SpriteRenderer _spriteRenderer;
    private GameObject _heldItem, _currentEmote, _fullHeart;
    private int _hitPoints;

    [Header("Setup")] 
    
    [SerializeField] private Transform groundPoint;
    [SerializeField] private LayerMask groundLayer, interactable;
    [SerializeField] private bool flipEnabled;
    [SerializeField] private CinemachineVirtualCamera cinemachine;
    [SerializeField] private GameObject itemSocket, emoteSocket, reviveTrigger;
    [SerializeField] private Animator spriteAnimator;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Parameters")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int hitPoints = 1;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        // for the sprite child
        _spriteRenderer = spriteAnimator.GetComponent<SpriteRenderer>();
        _playerOrientation = GetComponent<Animator>();
        _hitPoints = hitPoints;
        _fullHeart = GameManager.Instance.heartUI;
    }
    
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (!_isCamActive)
        {
            if (photonView.IsMine)
            {
                cinemachine.gameObject.SetActive(true);
                _isCamActive = true;
            }
        }
        
        if (!photonView.IsMine) return;
        if (_isDead) return;
        
        _moveInput.x = Input.GetAxis("Horizontal");
        _moveInput.y = Input.GetAxis("Vertical");
        _moveInput.Normalize();

        _rb.velocity = new Vector3(_moveInput.x * moveSpeed, _rb.velocity.y, _moveInput.y * moveSpeed);

        spriteAnimator.SetFloat("moveSpeed", _rb.velocity.magnitude);
        
        RaycastHit hit;

        if (Physics.Raycast(groundPoint.position, Vector3.down, out hit, .3f, groundLayer))
        {
            _isGrounded = true;
        }
        else _isGrounded = false;

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.velocity += new Vector3(0f, jumpForce, 0f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            var pickup = PickupItem();
            if (pickup == null) return;
            
            if (pickup.TryGetComponent(out ReviveBuoy reviveBuoy))
            {
                reviveBuoy.BuoyRevive();
                return;
            }

            if (pickup.TryGetComponent(out RisingPickup risingPickup))
            {
                if (risingPickup.CheckIfHeld()) return;
            }
            

            if (_heldItem != null) DropItem();
            
            _heldItem = pickup;
            var view = _heldItem.GetPhotonView();
            if (view != null) view.TransferOwnership(PhotonNetwork.LocalPlayer);
            AttachItem(_heldItem);
            
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            DropItem();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            UseItem();
        }
        

        if (Input.GetKeyDown(KeyCode.K))
        {
            // todo: allow the player to charge the throw
            
            if (ThrowItem(ref _heldItem, _rb.velocity)) audioSource.Play();
        }
        
        spriteAnimator.SetBool("onGround", _isGrounded);

        if (!_isFlipped && _moveInput.x < 0)
        {
            _isFlipped = true;
            //_flipX = true; // todo: this should be made observable somehow
            //_spriteRenderer.flipX = _flipX;
            spriteAnimator.SetBool("swapX", true);
            _spriteRenderer.flipX = spriteAnimator.GetBool("swapX");
            if(flipEnabled) _playerOrientation.SetTrigger("Flip");
        } else if (_isFlipped && _moveInput.x > 0)
        {
            _isFlipped = false;
            //_flipX = false;
            //_spriteRenderer.flipX = _flipX;
            spriteAnimator.SetBool("swapX", false);
            _spriteRenderer.flipX = spriteAnimator.GetBool("swapX");
            if(flipEnabled) _playerOrientation.SetTrigger("Flip");
        }

        if (!_isBackwards && _moveInput.y > 0)
        {
            _isBackwards = true;
            if(flipEnabled) _playerOrientation.SetTrigger("Flip");
        } else if (_isBackwards && _moveInput.y < 0)
        {
            _isBackwards = false;
            if(flipEnabled) _playerOrientation.SetTrigger("Flip");
        }

        spriteAnimator.SetBool("movingBackwards", _isBackwards);
    }

    private bool ThrowItem(ref GameObject heldItem, Vector3 direction)
    {
        if (heldItem == null) return false;

        if (heldItem.TryGetComponent(out RisingInteractable item))
        {
            heldItem.transform.parent = null;
            item.Throw(direction, _collider);
            _heldItem = null;
            return true;
        }

        return false;
    }

    private void UseItem()
    {
        if (_heldItem == null) return;

        if (_heldItem.TryGetComponent(out RisingInteractable item))
        {
            item.Interact();
            //photonView.RPC("");
        }
    }

    private void DropItem()
    {
        if (_heldItem == null) return;

        if (_heldItem.TryGetComponent(out RisingPickup item))
        {
            
            item.photonView.RPC("Drop", RpcTarget.AllBuffered);
            item.transform.parent = null;
            _heldItem = null;
        }

    }

    private void AttachItem(GameObject pickupItem)
    {
        if (pickupItem == null) return;
        
        
        if (pickupItem.TryGetComponent(out RisingPickup item))
        {
            // if (_heldItem != null) DropItem(_heldItem);
            // todo: drop items when you pick up a new item
            //item.Pickup();
            item.GetComponent<PhotonView>().RPC("Pickup", RpcTarget.AllBuffered);
            pickupItem.transform.position = itemSocket.transform.position;
            pickupItem.transform.parent = itemSocket.transform;
        } else Debug.LogWarning("tried to pickup item without Pickup component" + pickupItem);
        
    }

    // todo: this can be made generic
    private GameObject PickupItem()
    {
        /* Pickup the closest item */
        int maxColliders = 5;
        Collider[] hitColliders = new Collider[maxColliders];
        if (Physics.OverlapSphereNonAlloc(transform.position, 1, hitColliders, interactable,
                QueryTriggerInteraction.Collide) > 0)
        {
            
            // todo: this can be cleaned up no doubt
            var temp = hitColliders.ToList();
            
            temp.RemoveAll(x => x == null);
            // don't try to pickup the currently held item
            if (_heldItem != null) temp.Remove(_heldItem.GetComponent<Collider>());
            var distanceSorted = temp.OrderBy(x => (transform.position - x.transform.position).sqrMagnitude).ToList();

            if (distanceSorted.Count > 0)
            {
                return distanceSorted[0].gameObject;
            }
        }
        print("no item found");
        return null;
    }

    [ContextMenu("deadTest")]
    public void DeadTest()
    {
        /* turn on dead stuff */
        reviveTrigger.SetActive(_isDead);
        emoteSocket.SetActive(_isDead);
        spriteAnimator.SetBool("isDead", _isDead);
        if (_fullHeart!=null) _fullHeart.SetActive(!_isDead);
        
        if (_isDead)
        {
            DropItem();
        }
        // todo: the client cant delete this because it doesnt own it?! find another way to delete it
        // else // PhotonNetwork.Destroy(_currentEmote);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.Serialize(ref _isDead);
            stream.Serialize(ref _hitPoints);
        }
        else
        {
            stream.Serialize(ref _isDead);
            stream.Serialize(ref _hitPoints);
        }

    }
    public void Interact()
    {
    }

    public void Hit(Vector3 pos, float power)
    {
        
    }

    public void Throw(Vector3 direction, Collider playerCollider)
    {
        
    }

    public void ItemDestroyed()
    {
        
    }

    [PunRPC]
    public bool DamagePlayer()
    {
        if (_isDead) return false;
        
        if (_hitPoints > 0) _hitPoints--;

        if (_hitPoints == 0)
        {
            _isDead = true;
            DeadTest();
        }

        return true;
    }
    [PunRPC]
    public void RevivePlayer()
    {
        if (!_isDead) return;
        _isDead = false;
        DeadTest();
        _hitPoints = hitPoints;
    }
}
