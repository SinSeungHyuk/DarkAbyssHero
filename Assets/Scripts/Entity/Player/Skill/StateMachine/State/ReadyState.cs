using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyState : State<Skill>
{
    public override void Enter()
    {
        // TOwner = ��ų
        // ��ų�� Ȱ��ȭ�� �����ϰ� ��� ������Ƽ�� 0���� �ʱ�ȭ

        if (Layer == 0)
        {
            if (TOwner.IsActivated)
                TOwner.Deactivate();

            TOwner.ResetProperties();
        }
    }
}