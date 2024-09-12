using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingState : SkillState
{
    public override void Enter()
    {
        TOwner.Activate();
        TOwner.StartCustomActions(SkillCustomActionType.Cast);

        // 플레이어의 스테이트머신에게 캐스팅 상태로 전이 명령보내기
        TrySendCommandToPlayer(TOwner, PlayerStateCommand.ToCastingSkillState, TOwner.CastAnimationParameter);
    }

    public override void Update()
    {
        TOwner.CurrentCastTime += Time.deltaTime;
        TOwner.RunCustomActions(SkillCustomActionType.Cast);
    }

    public override void Exit()
        => TOwner.ReleaseCustomActions(SkillCustomActionType.Cast);
}
