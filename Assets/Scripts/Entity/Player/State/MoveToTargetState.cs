using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTargetState : State<Player>
{
    public override void Enter()
    {
        TOwner.SkillSystem.ReserveSkill.Use();
    }
}