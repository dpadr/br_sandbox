using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BR_HealthComponent : BR_Component
{
    [SerializeField]
    private int maxHealth;
    public int MaxHealth => maxHealth;

    private int _currentHealth;
    public int CurrentHealth => _currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = maxHealth;
    }

    public override void OnUpdate()
    {

    }

    public int TakeDamage(int damage)
    {
        _currentHealth -= damage;
        return _currentHealth;
    }
}
