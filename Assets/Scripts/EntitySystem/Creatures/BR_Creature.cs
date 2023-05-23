using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BR_HealthComponent))]
public abstract class BR_Creature : BR_Entity, IF_Damageable
{
    private BR_HealthComponent _healthComponent;

    private void Start()
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

    private void Die()
    {
        SetState(EntityState.Destroy);
    }
}
