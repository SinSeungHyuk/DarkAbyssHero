using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CooldownState : SkillState
{
    public override void Enter()
    {
        Debug.Log($"현재 쿨타임 : {TOwner.CurrentCooldown} , 스킬쿨타임 : {TOwner.Cooldown}");

        if (TOwner.IsActivated)
            TOwner.Deactivate();

        // 쿨타임이 모두 감소되었으면 다시 Cooldown으로 되돌려놓기
        if (TOwner.IsCooldownCompleted)
            TOwner.CurrentCooldown = TOwner.Cooldown;
    }

    public override void Update()
    {
        TOwner.CurrentCooldown -= Time.deltaTime;
    }
}