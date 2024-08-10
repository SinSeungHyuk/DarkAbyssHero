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
        // SkillState�� TrySendCommandToPlayer �Լ��� ���� �޼��� ���޵�
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill, int))data;

        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2;

        Debug.Assert(RunningSkill != null,
            $"CastingSkillState({message})::OnReceiveMessage - �߸��� data�� ���޵Ǿ����ϴ�.");

        // ĳ���� ������ Bool �Ķ���ͷ� ����
        TOwner.Animator?.SetBool(AnimatorParameterHash,true);

        return true;
    }
}
