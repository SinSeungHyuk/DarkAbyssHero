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
        // 현재 재생중인 애니메이션이 Locomotion 스테이트인지 확인하기 위한 변수
        LocomotionState = Settings.LocomotionState;
    }

    public override void Update()
    {
        //GetCurrentAnimatorStateInfo: 지금 애니메이션의 상태정보(Update에서 받아와야함)
        lastStateInfo = TOwner.Animator.GetCurrentAnimatorStateInfo(0);

        // 지금 실행중인 애니메이션 Hash == Locomotion -> 현재 Idle,Run 애니메이션 재생중
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
        // SkillState의 TrySendCommandToPlayer 함수를 통해 메세지 전달됨
        if ((EntityStateMessage)message == EntityStateMessage.FinishSkill)
        {
            // InActionState 에서 EntityStateMessage.FinishSkill 메세지가 전달되면
            IsSkillFinished = true;
            return true;
        }

        var tupleData = ((Skill, int))data;

        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2;

        Debug.Assert(RunningSkill != null,
            $"CastingSkillState({message})::OnReceiveMessage - 잘못된 data가 전달되었습니다.");

        TOwner.Animator?.SetTrigger(AnimatorParameterHash);

        return true;
    }
}
