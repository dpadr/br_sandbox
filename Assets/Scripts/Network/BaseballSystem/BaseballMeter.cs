using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseballMeter : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private RectTransform meterBody;
    private float _meterLength, _curMeterLength;

    private void Awake()
    {
        if (meterBody.TryGetComponent(out RectTransform rect))
        {
            _meterLength = _curMeterLength = rect.rect.width;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
