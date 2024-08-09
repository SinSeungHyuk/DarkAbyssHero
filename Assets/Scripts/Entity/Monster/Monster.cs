using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity, IDamageable
{
    [SerializeField] private Transform player;

    private EntityMovement movement;


    void Start()
    {
        movement = GetComponent<EntityMovement>();

        movement.SetUp(this);
    }

    void Update()
    {
        movement.TraceTarget = player;
    }


    #region Interface
    public void TakeDamage(float damage)
    {
        
    }
    public void OnDead()
    {
        
    }
    #endregion
}
