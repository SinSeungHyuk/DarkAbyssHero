using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FindUsableSkillState : State<Player>
{
    public override void Enter()
    {
        //SkillSystem skillSystem = TOwner.SkillSystem;

        //skillSystem.FindUsableSkill();

        //TOwner.Movement.StopDistance = reserveSkill.Distance;

        //TOwner.SkillSystem.ReserveSkill.Use();

        //Owner.ExecuteCommand(SkillExecuteCommand.Ready);

        Debug.Log(TOwner.Movement.StopDistance);

    }

    public override void Update()
    {
        Debug.Log(TOwner.Movement.IsStop);


    }
}
