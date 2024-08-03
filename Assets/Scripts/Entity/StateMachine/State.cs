using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State<TOwnerType>
{
    // TOwner는 State 소유자의 Type (Skill, Entity ...)
    // StateMachine의 TOwner과 일치해야함
    public StateMachine<TOwnerType> Owner { get; private set; }
    public TOwnerType TOwner { get; private set; }
    public int Layer { get; private set; }

    // 스테이트머신에서 사용할 함수
    public void SetUp(StateMachine<TOwnerType> owner, TOwnerType type, int layer)
    {
        Owner = owner;
        TOwner = type;
        Layer = layer;

        Awake();
    }

    // 각 스테이트의 변수 초기화 작업을 진행할 Awake 역할용
    protected virtual void Awake() { }

    // State가 시작될때
    public virtual void Enter() { }
    // State 실행중일때 프레임마다
    public virtual void Update() { }
    // State 나갈때
    public virtual void Exit() { }

    // StateMachine 클래스의 SendMessage 함수로 호출할 함수
    // 스테이트머신에서 이 스테이트에 특정 메세지(enum)을 보낼때 사용
    public virtual bool OnReceiveMessage(int message, object data)
        => false;
}
