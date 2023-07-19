using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks
{

    #region singleton

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject logo;
    [SerializeField] private AudioClip ambient;
    public GameObject heartUI;
    private AudioSource _source;
    private bool _showControls;

    /* Instead of using callback interfaces all over the place we can use the GameManager to route callbacks */
    public static event Action MasterClientChange;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisplayControls();
        }
    }

    private void DisplayControls()
    {
        controlsPanel.SetActive(_showControls);
        _showControls = !_showControls;
    }

    private void Start()
    {
        if (TryGetComponent(out AudioSource source))
        {
            _source = source;
        }
    }

    #region Photon Callbacks

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        // SceneManager.LoadScene(0);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    public override void OnJoinedRoom()
    {
        MasterClientChange?.Invoke();
        
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        }
        else
        {
            if (PlayerManager.localPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                var newPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,1f,0f), Quaternion.identity, 0);

                newPlayer.name = "Player: " + PhotonNetwork.NickName;
                if (logo != null) logo.SetActive(false);
                if (_source != null)
                {
                    _source.Stop();
                    _source.clip = ambient;
                    _source.volume = .15f;
                    _source.Play();
                }
            }
            else Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);

            
        }
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        MasterClientChange?.Invoke();
    }
    
}

