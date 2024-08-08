using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity, IDamageable
{
    [SerializeField] private Player player;

    private EntityMovement movement;


    void Start()
    {
        movement = GetComponent<EntityMovement>();

        movement.TraceTarget = player.transform;
    }

    void Update()
    {
        
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
