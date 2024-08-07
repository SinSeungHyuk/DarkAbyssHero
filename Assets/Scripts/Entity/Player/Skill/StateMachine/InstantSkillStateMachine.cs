using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class InstantSkillStateMachine : StateMachine<Skill>
{
    protected override void AddStates()
    {
        AddState<ReadyState>();
        AddState<CastingState>();
        AddState<InActionState>();
        AddState<CooldownState>();
    }

    protected override void MakeTransitions()
    {
        // Ready State -> ToState
        //
        // 스킬 Use 커맨드 && 이 스킬이 캐스팅을 사용하는 스킬이라면
        MakeTransition<ReadyState, CastingState>(SkillExecuteCommand.Use, state => TOwner.IsUseCast);
        // 위 조건을 넘어갔다는 뜻은 캐스팅이 아니라는 의미
        MakeTransition<ReadyState, InActionState>(SkillExecuteCommand.Use);
        // 쿨다운이 남아있다면 
        MakeTransition<ReadyState, CooldownState>(state => !TOwner.IsCooldownCompleted);


        // Casting State -> ToState
        //
        // 캐스팅 상태 내에서 Update를 통해 캐스팅 시간을 모두 채웠으면
        MakeTransition<CastingState, InActionState>(state => TOwner.IsCastCompleted);


        // InAction State -> ToState
        //
        // 스킬의 사용이 끝났고 쿨다운을 가지고 있다면 쿨다운상태로
        MakeTransition<InActionState,CooldownState>(state => TOwner.IsFinished &&  TOwner.HasCooldown); 
        // 스킬의 사용이 끝나고 쿨다운이 없는 스킬이라면 
        MakeTransition<InActionState,CooldownState>(state => TOwner.IsFinished); 


        // Cooldown State -> ToState
        //
        // 쿨다운이 모두 완료되었으면 준비상태로
        MakeTransition<CooldownState,ReadyState>(state=> TOwner.IsCooldownCompleted);



        // 필요에 따라서 MakeAnyTransition 으로 스킬캔슬시 쿨다운 스테이트로 돌입 만들기
    }
}