using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingSkillState : PlayerSkillState
{
    public override void Exit()
    {
        TOwner.Animator?.SetBool(AnimatorParameterHash, false);
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // SkillState의 TrySendCommandToPlayer 함수를 통해 메세지 전달됨
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill, int))data;

        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2;

        Debug.Assert(RunningSkill != null,
            $"CastingSkillState({message})::OnReceiveMessage - 잘못된 data가 전달되었습니다.");

        // 캐스팅 동작은 Bool 파라미터로 동작
        TOwner.Animator?.SetBool(AnimatorParameterHash,true);

        return true;
    }
}
