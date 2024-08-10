using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyState : State<Skill>
{
    public override void Enter()
    {
        // TOwner = ��ų
        // ��ų�� Ȱ��ȭ�� �����ϰ� ��� ������Ƽ�� 0���� �ʱ�ȭ

        if (TOwner.IsActivated)
            TOwner.Deactivate();

        TOwner.ResetProperties();
    }

    public override void Exit()
    {
    }
}