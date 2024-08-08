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
        AddState<MoveToTargetState>();

        AddState<CastingSkillState>();
        AddState<InSkillActionState>();

        AddState<DeadState>();
    }

    protected override void MakeTransitions()
    {
        MakeTransition<DetectMonsterState, FindUsableSkillState>(state => (state as DetectMonsterState).IsFindSkill);
        ////MakeTransition<FindUsableSkillState, MoveToTargetState>(SkillExecuteCommand.Ready);

        MakeTransition<DetectMonsterState, CastingSkillState>(PlayerStateCommand.ToCastingSkillState);
        MakeTransition<DetectMonsterState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);

        MakeTransition<MoveToTargetState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        MakeTransition<CastingSkillState, InSkillActionState>(PlayerStateCommand.ToInSkillActionState);
        
        //MakeTransition<InSkillActionState, DetectMonsterState>(state => !IsSkillInState<InActionState>(state));
        MakeTransition<InSkillActionState, MoveToTargetState>(state => (state as InSkillActionState).IsStateEnded);

    }
    

    private bool IsSkillInState<T>(State<Player> state) where T : State<Skill>
        => (state as PlayerSkillState).RunningSkill.IsInState<T>();
}
