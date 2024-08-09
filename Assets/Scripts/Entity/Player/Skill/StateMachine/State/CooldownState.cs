using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownState : SkillState
{
    public override void Enter()
    {
        if (TOwner.IsActivated)
            TOwner.Deactivate();

        // ��Ÿ���� ��� ���ҵǾ����� �ٽ� Cooldown���� �ǵ�������
        if (TOwner.IsCooldownCompleted)
            TOwner.CurrentCooldown = TOwner.Cooldown;

        Debug.Log($"���� ��Ÿ�� : {TOwner.CurrentCooldown} , ��ų��Ÿ�� : {TOwner.Cooldown}");

    }

    public override void Update()
    {
        TOwner.CurrentCooldown -= Time.deltaTime;
    }

    public override void Exit()
    {
        Debug.Log($"��ٿ� Exit!!!! ���� ��Ÿ�� : {TOwner.CurrentCooldown} , ��ų��Ÿ�� : {TOwner.Cooldown}");
    }
}