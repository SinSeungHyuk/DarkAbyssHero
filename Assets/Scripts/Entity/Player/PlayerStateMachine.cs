using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerStateMachine : EntityStateMachine<Player>
{
    protected override void AddStates()
    {
        AddState<DetectMonsterState>();
        AddState<FindUsableSkillState>();
        //AddState<MoveToTargetState>();

        AddState<CastingSkillState>();
        AddState<InSkillActionState>();

        AddState<DeadState>();
    }

    protected override void MakeTransitions()
    {
        MakeTransition<DetectMonsterState, FindUsableSkillState>(SkillExecuteCommand.Find);
        //MakeTransition<FindUsableSkillState, MoveToTargetState>(SkillExecuteCommand.Ready);

        MakeTransition<FindUsableSkillState, CastingSkillState>(PlayerStateCommand.ToCastingSkillState);
        MakeTransition<FindUsableSkillState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        MakeTransition<CastingSkillState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        MakeTransition<InSkillActionState, DetectMonsterState>(state => (state as InSkillActionState).IsStateEnded);
    }
}
