using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity, IDamageable
{
    [SerializeField] private Transform player;

    private EntityMovement movement;
    private EffectSystem effectSystem;

    public EffectSystem EffectSystem => effectSystem;


    void Start()
    {
        movement = GetComponent<EntityMovement>();
        effectSystem = GetComponent<EffectSystem>();

        movement.SetUp(this);
    }

    void Update()
    {
        movement.TraceTarget = player;
    }

    private void OnDestroy()
    {

    }


    #region Interface
    public void TakeDamage(float damage)
    {
        Debug.Log($"{gameObject.name} + TakeDamage : {damage}");
    }
    public void OnDead()
    {
        
    }
    #endregion
}
