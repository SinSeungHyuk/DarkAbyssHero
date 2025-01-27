using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSkillState : State<Player>
{
    // 현재 Entity가 실행중인 Skill (애니메이션으로 스킬적용을 위해 필요)
    public Skill RunningSkill { get;  set; }
    // Entity가 실행해야할 Animation의 Hash
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
        // SkillState의 TrySendCommandToPlayer 함수를 통해 메세지 전달됨
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill, int))data;

        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2;

        Debug.Assert(RunningSkill != null,
            $"CastingSkillState({message})::OnReceiveMessage - 잘못된 data가 전달되었습니다.");

        TOwner.Animator?.SetTrigger(AnimatorParameterHash);

        return true;
    }
}
