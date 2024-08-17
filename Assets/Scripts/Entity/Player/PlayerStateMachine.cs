using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : EntityStateMachine<Player>
{
    protected override void AddStates()
    {
        AddState<DetectMonsterState>();
        AddState<MoveToTargetState>();
        AddState<EmptyState>();

        AddState<CastingSkillState>();
        AddState<InSkillActionState>();

        AddState<DeadState>();
    }

    protected override void MakeTransitions()
    {
        // Detect -> MoveToTarget
        MakeTransition<DetectMonsterState, MoveToTargetState>(state => (state as DetectMonsterState).IsFindSkill == true);

        // MoveToTarget -> Casting or InSkillAction
        MakeTransition<MoveToTargetState, CastingSkillState>(PlayerStateCommand.ToCastingSkillState);
        MakeTransition<MoveToTargetState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        // Casting -> InSkillAction
        MakeTransition<CastingSkillState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        // InSkillAction -> Detect or Empty
        MakeTransition<InSkillActionState, DetectMonsterState>(state => (state as InSkillActionState).IsStateEnded 
            && (state as InSkillActionState).IsSkillFinished);
        MakeTransition<InSkillActionState, EmptyState>(state => (state as InSkillActionState).IsStateEnded);

        // Empty -> InSkillAction
        MakeTransition<EmptyState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
    }

    
    public bool IsSkillInState<T>(State<Player> state) where T : State<Skill>
        => (state as PlayerSkillState).RunningSkill.IsInState<T>();
}
