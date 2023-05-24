using System;
using Photon.Pun;

public class BaseballBlueprint : MonoBehaviourPun, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new NotImplementedException();
    }
}