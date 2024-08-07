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
        // �ùٸ� Message�� �ƴ϶�� false�� return
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
    //        // Skill�� �ѹ��̶� ����Ǿ��ٸ� State�� ����
    //        case InSkillActionFinishOption.FinishOnceApplied:
    //            IsStateEnded = true;
    //            break;

    //        // Skill�� ��� ����Ǿ��ٸ� State�� ����
    //        case InSkillActionFinishOption.FinishWhenFullyApplied:
    //            IsStateEnded = skill.IsFinished;
    //            break;
    //    }
    //}
}
