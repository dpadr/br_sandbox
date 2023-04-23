

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TMP_Text))]

/* Use IPunObservable to keep data updated across the network */
public class PlayerSetHandle : MonoBehaviourPunCallbacks, IPunObservable
{
    // Start is called before the first frame update
    public string _playerName;
    private TMP_Text nameText;
    private Color _currentColor;

    [SerializeField] private Color team1, team2;
    
    void Start()
    {
        nameText = GetComponent<TMP_Text>();
        /* We store the nickname of the local player */
        if (photonView.IsMine)
        {
            _playerName = nameText.text = PhotonNetwork.NickName;
            
            if (PhotonNetwork.CurrentRoom.PlayerCount % 2 == 0)
            {
                _currentColor = team1;
                UpdateColor();
            }
            else
            {
                _currentColor = team2;
                UpdateColor();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /* This callback reads/writes any variables we want */
        if (stream.IsWriting)
        {
            stream.SendNext(_playerName);
            stream.SendNext(new Vector3(_currentColor.r, _currentColor.g, _currentColor.b));
        } else if (stream.IsReading)
        {
            /* We read out the variable supplied by the player */
            _playerName = (string) stream.ReceiveNext();
            /* And update the presentation */
            nameText.text = _playerName;
            
            var temp = (Vector3) stream.ReceiveNext();
            _currentColor = new Color(temp.x, temp.y, temp.z, 100);
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        nameText.color = _currentColor;
    }
}
