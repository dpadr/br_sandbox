using System.Linq;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.InputSystem;

public class BR_PlayerController : BR_Creature
{
    private Rigidbody _rb;
    private Collider _collider;
    [HideInInspector] public Vector2 _moveInput;
    [HideInInspector] public bool _isGrounded, _isFlipped, _isBackwards, _isCamActive, _flipX, _isDead;
    [HideInInspector] public Animator _playerOrientation;
    [HideInInspector] public SpriteRenderer _spriteRenderer;
    [HideInInspector] public GameObject _heldItem, _currentEmote, _fullHeart;
    private int _hitPoints;

    [Header("Setup")] 
    
    [SerializeField] public Transform groundPoint;
    [SerializeField] public LayerMask groundLayer, interactable;
    [SerializeField] public bool flipEnabled;
    [SerializeField] public CinemachineVirtualCamera cinemachine;
    [SerializeField] public GameObject itemSocket, emoteSocket, reviveTrigger;
    [SerializeField] public Animator spriteAnimator;
    [SerializeField] public AudioSource audioSource;
    
    [Header("Parameters")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int hitPoints = 1;

    [Header("Input Actions")]
    public PlayerInputActions playerActions;
    [HideInInspector] public InputAction movement, jump, pickup, drop, pitch, interact;


    public enum PlayerState
    {
        Running,
        Jumping,
        Falling,
        Sliding,
        Idle,
        Incapacitated,
        Dead
    }

    [HideInInspector] public BRL_Idle Idle;
    [HideInInspector] public BRL_Running Running;
    [HideInInspector] public BRL_Jumping Jumping;
    [HideInInspector] public BRL_Dead Dead;
    [HideInInspector] public BRL_Falling Falling;
    [HideInInspector] public BRL_Sliding Sliding;
    [HideInInspector] public BRL_Incapacitated Incapacitated;

    public BRL_BaseState currentState;
    public PlayerState currentStateName;

    private void Awake() {
        // set up states
        Idle = new BRL_Idle();
        Running = new BRL_Running();
        Jumping = new BRL_Jumping();
        Dead = new BRL_Dead();
        Falling = new BRL_Falling();
        Sliding = new BRL_Sliding();
        Incapacitated = new BRL_Incapacitated();
        changeState(Idle);
        
        // set up inputs
        playerActions = new PlayerInputActions();
        movement = playerActions.Player.Movement;
        jump = playerActions.Player.Jump;
        pickup = playerActions.Player.Pickup;
        drop = playerActions.Player.Drop;
        pitch = playerActions.Player.Throw;
        interact = playerActions.Player.Interact;
    }

    private void OnEnable() {
        playerActions.Player.Enable();
    }

    private void OnDisable() {
        playerActions.Player.Disable();
    }


    // Start is called before the first frame update
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

    // Update is called once per frame
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
        //if (_isDead) return;

        // todo
        OnUpdate();
    }

    protected override void OnUpdate(){
        currentState.OnUpdate();
    }

    public void changeState(BRL_BaseState newState) {
        if (currentStateName == newState.stateName) {
            // same state, do need to change
            return;
        }


        if (currentState != null)
        {
            // need to exit current state
            currentState.OnExit();
        }

        // need to enter current state
        currentState = newState;
        currentStateName = newState.stateName;
        newState.OnEnter(this);
    }

    public float getMoveSpeed() {
        return moveSpeed;
    }

    public float getJumpForce() {
        return jumpForce;
    }

    // copied over from old playercontroller
    public GameObject PickupItem()
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

    public bool ThrowItem(ref GameObject heldItem, Vector3 direction)
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

    public void UseItem()
    {
        if (_heldItem == null) return;

        if (_heldItem.TryGetComponent(out RisingInteractable item))
        {
            item.Interact();
            //photonView.RPC("");
        }
    }

    public void DropItem()
    {
        if (_heldItem == null) return;

        if (_heldItem.TryGetComponent(out RisingPickup item))
        {
            
            item.photonView.RPC("Drop", RpcTarget.AllBuffered);
            item.transform.parent = null;
            _heldItem = null;
        }

    }

    public void AttachItem(GameObject pickupItem)
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
