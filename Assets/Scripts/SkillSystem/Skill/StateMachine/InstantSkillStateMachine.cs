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
        // ��ų Use Ŀ�ǵ� && �� ��ų�� ĳ������ ����ϴ� ��ų�̶��
        MakeTransition<ReadyState, CastingState>(SkillExecuteCommand.Use, state => TOwner.IsUseCast);
        // �� ������ �Ѿ�ٴ� ���� ĳ������ �ƴ϶�� �ǹ�
        MakeTransition<ReadyState, InActionState>(SkillExecuteCommand.Use);


        // Casting State -> ToState
        //
        // ĳ���� ���� ������ Update�� ���� ĳ���� �ð��� ��� ä������
        MakeTransition<CastingState, InActionState>(state => TOwner.IsCastCompleted);


        // InAction State -> ToState
        //
        // ��ų�� ����� ������ ��ٿ��� ������ �ִٸ� ��ٿ���·�
        MakeTransition<InActionState, CooldownState>(state => TOwner.IsFinished && TOwner.HasCooldown);


        // Cooldown State -> ToState
        //
        // ��ٿ��� ��� �Ϸ�Ǿ����� �غ���·�
        MakeTransition<CooldownState, ReadyState>(state => TOwner.IsCooldownCompleted);



        // �ʿ信 ���� MakeAnyTransition ���� ��ųĵ���� ��ٿ� ������Ʈ�� ���� �����
        MakeAnyTransition<ReadyState>(SkillExecuteCommand.Cancel);
    }
}