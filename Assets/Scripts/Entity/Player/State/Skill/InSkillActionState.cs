using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InSkillActionState : PlayerSkillState
{
    public bool IsStateEnded { get; private set; }

    public override void Update()
    {
        IsStateEnded = RunningSkill.IsFinished;
        //IsStateEnded = !TOwner.Animator.GetBool(AnimatorParameterHash);
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // 올바른 Message가 아니라면 false를 return
        if (!base.OnReceiveMessage(message, data))
            return false;

        return true;
    }

    public override void Exit()
    {
        IsStateEnded = false;
        //RunningSkill.onApplied -= OnSkillApplied;

        base.Exit();
    }

    //private void OnSkillApplied(Skill skill, int currentApplyCount)
    //{
    //    switch (skill.InSkillActionFinishOption)
    //    {
    //        // Skill이 한번이라도 적용되었다면 State를 종료
    //        case InSkillActionFinishOption.FinishOnceApplied:
    //            IsStateEnded = true;
    //            break;

    //        // Skill이 모두 적용되었다면 State를 종료
    //        case InSkillActionFinishOption.FinishWhenFullyApplied:
    //            IsStateEnded = skill.IsFinished;
    //            break;
    //    }
    //}
}
