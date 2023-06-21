using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Image = UnityEngine.UI.Image;

// [ExecuteInEditMode]
public class BaseballMeter : MonoBehaviourPun
{
    [SerializeField] private Image progressMask, fill;
    private int _max, _current;
    private int Current
    {
        get => _current;
        set
        {
            if (value != _current)
            {
                _current = value;
                OnValueChanged();
            }
        }
    }

    private void OnValueChanged() // cool place to do some logic
    {
        progressMask.fillAmount = (float)Current / (float)_max;
        
        if (Current > 50) fill.color = Color.green;
        else if (Current <= 50 && Current > 25) fill.color = Color.yellow;
        else if (Current <= 25) fill.color = Color.red;
    }

    private void GetCurrentFill(int amount)
    {
        Current = amount;
    }
    
    private void OnEnable()
    {
        BaseballManager.UpdateBballMeter += GetCurrentFill;
        
        Current = BaseballManager.Instance.MeterDefaultMax.x;
        _max = BaseballManager.Instance.MeterDefaultMax.y;
    }

    private void OnDisable()
    {
        BaseballManager.UpdateBballMeter -= GetCurrentFill;
        
    }

}
