using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CooldownState : SkillState
{
    public override void Enter()
    {
        Debug.Log($"���� ��Ÿ�� : {TOwner.CurrentCooldown} , ��ų��Ÿ�� : {TOwner.Cooldown}");

        if (TOwner.IsActivated)
            TOwner.Deactivate();

        // ��Ÿ���� ��� ���ҵǾ����� �ٽ� Cooldown���� �ǵ�������
        if (TOwner.IsCooldownCompleted)
            TOwner.CurrentCooldown = TOwner.Cooldown;
    }

    public override void Update()
    {
        TOwner.CurrentCooldown -= Time.deltaTime;
    }
}