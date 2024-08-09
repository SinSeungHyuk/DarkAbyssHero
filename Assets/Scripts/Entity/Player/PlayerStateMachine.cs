using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerStateMachine : EntityStateMachine<Player>
{
    protected override void AddStates()
    {
        AddState<DetectMonsterState>();
        AddState<MoveToTargetState>();
        AddState<EmptyState>();
        AddState<SkillFinishedState>();

        AddState<CastingSkillState>();
        AddState<InSkillActionState>();

        AddState<DeadState>();
    }

    protected override void MakeTransitions()
    {
        MakeTransition<DetectMonsterState, MoveToTargetState>(state => (state as DetectMonsterState).IsFindSkill == true);

        MakeTransition<MoveToTargetState, CastingSkillState>(PlayerStateCommand.ToCastingSkillState);
        MakeTransition<MoveToTargetState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        MakeTransition<CastingSkillState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        MakeTransition<InSkillActionState, DetectMonsterState>(state => (state as InSkillActionState).IsStateEnded && (state as InSkillActionState).IsSkillFinished);
        MakeTransition<InSkillActionState, EmptyState>(state => (state as InSkillActionState).IsStateEnded);

        MakeTransition<EmptyState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
    }

    
    public bool IsSkillInState<T>(State<Player> state) where T : State<Skill>
        => (state as PlayerSkillState).RunningSkill.IsInState<T>();
}
