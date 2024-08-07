using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTargetState : State<Player>
{
    private EntityMovement movement;
    private Skill skill;

    protected override void Awake()
    {
        movement = TOwner.Movement;
    }

    public override void Enter()
    {
        Debug.Log("MoveToTargetState  EnterEnterEnterEnter");
        skill = TOwner.SkillSystem.ReserveSkill;
    }

    public override void Exit()
    {
        //skill.Use();
        Debug.Log("exit");
    }
}