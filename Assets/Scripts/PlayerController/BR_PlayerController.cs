using System.Collections;
using System.Collections.Generic;
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
    private GameObject _heldItem, _currentEmote, _fullHeart;
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
    [SerializeField] public InputActionReference movement;
    [SerializeField] public InputActionReference jump;


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

    public BRL_BaseState currentState;
    public PlayerState currentStateName;

    private void Awake() {
        Idle = new BRL_Idle();
        Running = new BRL_Running();
        Jumping = new BRL_Jumping();
        changeState(Idle);
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

        // todo
        OnUpdate();
    }

    protected override void OnUpdate(){
        currentState.OnUpdate();
    }

    public void changeState(BRL_BaseState newState) {
        if (currentState != null)
        {
            currentState.OnExit();
        }
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

}
