using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<Player>
{
    private EntityMovement movement;
    protected override void Awake()
    {
        movement = TOwner.GetComponent<EntityMovement>();
    }

    public override void Enter()
    {
        if (movement)
        {
            movement.Stop();
            movement.enabled = false;
        }
    }

    public override void Exit()
    {
        if (movement)
            movement.enabled = true;
    }

}
