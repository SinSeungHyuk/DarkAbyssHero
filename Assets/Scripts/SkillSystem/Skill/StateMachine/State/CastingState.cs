using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingState : SkillState
{
    public override void Enter()
    {
        TOwner.Activate();
        TOwner.StartCustomActions(SkillCustomActionType.Cast);

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
