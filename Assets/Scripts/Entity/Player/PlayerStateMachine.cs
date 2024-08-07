using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerStateMachine : EntityStateMachine<Player>
{
    protected override void AddStates()
    {
        AddState<FindUsableSkillState>();
        AddState<DetectMonsterState>();
        AddState<MoveToTargetState>();

        AddState<CastingSkillState>();
        AddState<InSkillActionState>();

        AddState<DeadState>();
    }

    protected override void MakeTransitions()
    {
        MakeTransition<FindUsableSkillState, DetectMonsterState>(SkillExecuteCommand.Find);
        MakeTransition<DetectMonsterState, MoveToTargetState>(SkillExecuteCommand.Ready);

        MakeTransition<MoveToTargetState, CastingSkillState>(PlayerStateCommand.ToCastingSkillState);
        MakeTransition<MoveToTargetState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);

        MakeTransition<CastingSkillState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);

        MakeTransition<InSkillActionState, FindUsableSkillState>(state => (state as InSkillActionState).IsStateEnded);
    }

    //private bool IsSkillInState<T>(State<Entity> state) where T : State<Skill>
    //    => (state as EntitySkillState).RunningSkill.IsInState<T>();
}
