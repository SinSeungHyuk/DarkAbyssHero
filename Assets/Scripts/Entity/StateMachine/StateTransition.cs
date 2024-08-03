using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StateTransition<TOwnerType>
{
    // 트랜지션의 커맨드가 없을때
    public const int NullCommand = int.MinValue;

    // 전이를 위한 조건 함수
    // Func<T1,T2> : T1을 매개변수로 T2를 반환 (Func 함수의 결과 bool 반환)
    private Func<State<TOwnerType>, bool> transitionCondition;

    // 현재 스테이트에서 다시 현재 스테이트로 전이 가능한지 여부
    public bool CanTransitionToSelf { get; private set; }
    // 현재 스테이트
    public State<TOwnerType> FromState { get; private set; }
    // FromState로부터 전이할 스테이트
    public State<TOwnerType> ToState { get; private set; }
    // 전이 명령어 (enum으로 사용할 예정)
    public int TransitionCommand { get; private set; }
    // 전이 가능한지 여부 (전이함수가 없거나 FromState로 가는게 true일 경우)
    public bool IsTransferable => transitionCondition == null || transitionCondition.Invoke(FromState);


    // 트랜지션 생성자
    public StateTransition(State<TOwnerType> fromState,
       State<TOwnerType> toState, 
       int transitionCommand,
       Func<State<TOwnerType>, bool> transitionCondition,
       bool canTransitionToSelf)
    {
        Debug.Assert(transitionCommand != NullCommand || transitionCondition != null,
            "StateTransition - TransitionCommand와 TransitionCondition은 둘 다 null이 될 수 없습니다.");

        FromState = fromState;
        ToState = toState;
        TransitionCommand = transitionCommand;
        this.transitionCondition = transitionCondition;
        CanTransitionToSelf = canTransitionToSelf;
    }
}
