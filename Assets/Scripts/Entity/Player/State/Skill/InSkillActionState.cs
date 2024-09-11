using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSkillActionState : PlayerSkillState
{
    private AnimatorStateInfo lastStateInfo;
    private int LocomotionState;

    public bool IsStateEnded { get; private set; }
    public bool IsSkillFinished { get; private set; }


    protected override void Awake()
    {
        // ���� ������� �ִϸ��̼��� Locomotion ������Ʈ���� Ȯ���ϱ� ���� ����
        LocomotionState = Settings.LocomotionState;
    }

    public override void Update()
    {
        //GetCurrentAnimatorStateInfo: ���� �ִϸ��̼��� ��������(Update���� �޾ƿ;���)
        lastStateInfo = TOwner.Animator.GetCurrentAnimatorStateInfo(0);

        // ���� �������� �ִϸ��̼� Hash == Locomotion -> ���� Idle,Run �ִϸ��̼� �����
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
            // InActionState ���� EntityStateMessage.FinishSkill �޼����� ���޵Ǹ�
            IsSkillFinished = true;
            return true;
        }

        var tupleData = ((Skill, int))data;

        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2;

        // �޾ƿ� �Ķ���ʹ� ��ų�ִϸ��̼� -> Ʈ���� �Ķ����
        TOwner.Animator?.SetTrigger(AnimatorParameterHash);

        return true;
    }
}
