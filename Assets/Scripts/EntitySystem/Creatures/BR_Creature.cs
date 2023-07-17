using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BR_HealthComponent))]
public abstract class BR_Creature : BR_Entity, IF_Damageable
{
    protected BR_HealthComponent _healthComponent;

    protected void Start()
    {
        _healthComponent = GetComponent<BR_HealthComponent>();
        AddComponent(_healthComponent, this);
    }
    public bool TakeDamage(int damage)
    {
        int remainingHealth = _healthComponent.TakeDamage(damage);
        if(remainingHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    protected void Die()
    {
        SetState(EntityState.Destroy);
    }
}
