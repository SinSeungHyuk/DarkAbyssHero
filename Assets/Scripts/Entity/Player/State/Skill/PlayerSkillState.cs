using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillState : State<Player>
{
    // ���� Entity�� �������� Skill (�ִϸ��̼����� ��ų������ ���� �ʿ�)
    public Skill RunningSkill { get;  set; }
    // Entity�� �����ؾ��� Animation�� Hash
    protected int AnimatorParameterHash { get;  set; }

    public override void Enter()
    {
        TOwner.Movement.Stop();
    }

    public override void Exit()
    {
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

        TOwner.Animator?.SetTrigger(AnimatorParameterHash);

        return true;
    }
}
