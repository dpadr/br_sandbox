

using System;
using Photon.Pun;
using UnityEngine;

public class BR_Tee : BR_Structure
{
    [SerializeField] private float coolDown = 5;

    private float _timeRemaining;
    private bool _isBallPresent, _isTimerRunning;

    private GameObject _currentBall;
    
    private void SpawnBall()
    {
        var position = transform.position + new Vector3(0, 1, 0);
        _currentBall = PhotonNetwork.InstantiateRoomObject("skull_ball", position, Quaternion.identity);
    }

    private void CooldownTimer()
    {
        if (_isBallPresent) return;
        
        if ( _timeRemaining > 0) _timeRemaining -= Time.deltaTime;
        else
        {
            _isBallPresent = true;
            SpawnBall();
            _timeRemaining = coolDown;
        }
    }
    
    protected override void OnUpdate()
    {
        if (!PhotonNetwork.InRoom) return;
        
        CooldownTimer();        
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject == _currentBall) _isBallPresent = false;
    }
}