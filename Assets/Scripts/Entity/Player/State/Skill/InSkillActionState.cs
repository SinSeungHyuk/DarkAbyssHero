using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSkillActionState : PlayerSkillState
{
    public bool IsStateEnded { get; private set; }
    public bool IsSkillFinished { get; private set; }
    private AnimatorStateInfo lastStateInfo;
    private int LocomotionState;


    protected override void Awake()
    {
        LocomotionState = Settings.LocomotionState;
    }

    public override void Update()
    {
        lastStateInfo = TOwner.Animator.GetCurrentAnimatorStateInfo(0);

        if (lastStateInfo.shortNameHash == LocomotionState && IsStateEnded == false)
        {
            IsStateEnded = true;
        }
    }

    public override void Exit()
    {
        IsStateEnded = false;
        IsSkillFinished = false;

        base.Exit();
    }


    public override bool OnReceiveMessage(int message, object data)
    {
        // SkillState�� TrySendCommandToPlayer �Լ��� ���� �޼��� ���޵�
        if ((EntityStateMessage)message == EntityStateMessage.FinishSkill)
        {
            IsSkillFinished = true;
            return true;
        }

        var tupleData = ((Skill, int))data;

        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2;

        Debug.Assert(RunningSkill != null,
            $"CastingSkillState({message})::OnReceiveMessage - �߸��� data�� ���޵Ǿ����ϴ�.");

        Debug.Log("OnReceiveMessage : " + AnimatorParameterHash);

        TOwner.Animator?.SetTrigger(AnimatorParameterHash);

        return true;
    }
}
