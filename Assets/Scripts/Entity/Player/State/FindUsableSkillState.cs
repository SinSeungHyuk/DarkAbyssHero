using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindUsableSkillState : State<Player>
{
    public override void Enter()
    {
        SkillSystem skillSystem = TOwner.GetComponent<SkillSystem>();
        Skill reserveSkill = skillSystem.FindUsableSkill();

        if (reserveSkill != null)
            skillSystem.ReserveSkill = reserveSkill;
        else skillSystem.ReserveSkill = skillSystem.DefaultSkill;


        Owner.ExecuteCommand(SkillExecuteCommand.Find);
    }
}
