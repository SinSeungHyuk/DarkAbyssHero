using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StateTransition<TOwnerType>
{
    // Ʈ�������� Ŀ�ǵ尡 ������
    public const int NullCommand = int.MinValue;

    // ���̸� ���� ���� �Լ�
    // Func<T1,T2> : T1�� �Ű������� T2�� ��ȯ (Func �Լ��� ��� bool ��ȯ)
    private Func<State<TOwnerType>, bool> transitionCondition;

    // ���� ������Ʈ���� �ٽ� ���� ������Ʈ�� ���� �������� ����
    public bool CanTransitionToSelf { get; private set; }
    // ���� ������Ʈ
    public State<TOwnerType> FromState { get; private set; }
    // FromState�κ��� ������ ������Ʈ
    public State<TOwnerType> ToState { get; private set; }
    // ���� ��ɾ� (enum���� ����� ����)
    public int TransitionCommand { get; private set; }
    // ���� �������� ���� (�����Լ��� ���ų� FromState�� ���°� true�� ���)
    public bool IsTransferable => transitionCondition == null || transitionCondition.Invoke(FromState);


    // Ʈ������ ������
    public StateTransition(State<TOwnerType> fromState,
       State<TOwnerType> toState, 
       int transitionCommand,
       Func<State<TOwnerType>, bool> transitionCondition,
       bool canTransitionToSelf)
    {
        Debug.Assert(transitionCommand != NullCommand || transitionCondition != null,
            "StateTransition - TransitionCommand�� TransitionCondition�� �� �� null�� �� �� �����ϴ�.");

        FromState = fromState;
        ToState = toState;
        TransitionCommand = transitionCommand;
        this.transitionCondition = transitionCondition;
        CanTransitionToSelf = canTransitionToSelf;
    }
}
