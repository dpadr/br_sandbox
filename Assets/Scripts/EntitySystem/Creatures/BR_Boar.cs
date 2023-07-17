using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_Boar : BR_Animal
{
    [SerializeField]
    private int maxHealth = 10;

    protected new void Start()
    {
        base.Start(); //calls the start function of the parent class (in this case, bc Animal has no start, it calls Creature's
        _healthComponent.SetMaxHealth(maxHealth);
    }
    protected override void OnUpdate()
    {

    }
}