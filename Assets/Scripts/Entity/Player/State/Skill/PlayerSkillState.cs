using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillState : State<Player>
{
    // ���� Entity�� �������� Skill
    public Skill RunningSkill { get; private set; }
    // Entity�� �����ؾ��� Animation�� Hash
    protected int AnimatorParameterHash { get; private set; }

    public override void Enter()
    {
        TOwner.Movement.Stop();
    }

    public override void Exit()
    {
        TOwner.Animator?.SetBool(AnimatorParameterHash, false);

        RunningSkill = null;
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


        TOwner.Animator?.SetBool(AnimatorParameterHash, true);

        return true;
    }
}
