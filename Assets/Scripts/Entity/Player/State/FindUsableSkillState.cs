using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FindUsableSkillState : State<Player>
{
    public override void Enter()
    {
        SkillSystem skillSystem = TOwner.SkillSystem;
        Skill reserveSkill = skillSystem.FindUsableSkill();

        if (reserveSkill != null)
            skillSystem.ReserveSkill = reserveSkill;
        else skillSystem.ReserveSkill = skillSystem.DefaultSkill;

        TOwner.Movement.StopDistance = reserveSkill.Distance;

        TOwner.SkillSystem.ReserveSkill.Use();

        //Owner.ExecuteCommand(SkillExecuteCommand.Ready);
    }
}
