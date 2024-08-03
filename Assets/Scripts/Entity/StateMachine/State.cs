using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State<TOwnerType>
{
    // TOwner�� State �������� Type (Skill, Entity ...)
    // StateMachine�� TOwner�� ��ġ�ؾ���
    public StateMachine<TOwnerType> Owner { get; private set; }
    public TOwnerType TOwner { get; private set; }
    public int Layer { get; private set; }

    // ������Ʈ�ӽſ��� ����� �Լ�
    public void SetUp(StateMachine<TOwnerType> owner, TOwnerType type, int layer)
    {
        Owner = owner;
        TOwner = type;
        Layer = layer;

        Awake();
    }

    // �� ������Ʈ�� ���� �ʱ�ȭ �۾��� ������ Awake ���ҿ�
    protected virtual void Awake() { }

    // State�� ���۵ɶ�
    public virtual void Enter() { }
    // State �������϶� �����Ӹ���
    public virtual void Update() { }
    // State ������
    public virtual void Exit() { }

    // StateMachine Ŭ������ SendMessage �Լ��� ȣ���� �Լ�
    // ������Ʈ�ӽſ��� �� ������Ʈ�� Ư�� �޼���(enum)�� ������ ���
    public virtual bool OnReceiveMessage(int message, object data)
        => false;
}
