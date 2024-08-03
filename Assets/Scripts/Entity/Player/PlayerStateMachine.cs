using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerStateMachine : EntityStateMachine<Player>
{
    protected override void AddStates()
    {
        
    }

    protected override void MakeTransitions()
    {
        
    }

    //private bool IsSkillInState<T>(State<Entity> state) where T : State<Skill>
    //=> (state as EntitySkillState).RunningSkill.IsInState<T>();
}
