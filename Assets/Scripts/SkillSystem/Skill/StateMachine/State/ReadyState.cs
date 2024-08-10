using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyState : State<Skill>
{
    public override void Enter()
    {
        // TOwner = 스킬
        // 스킬의 활성화를 종료하고 모든 프로퍼티를 0으로 초기화

        if (TOwner.IsActivated)
            TOwner.Deactivate();

        TOwner.ResetProperties();
    }

    public override void Exit()
    {
    }
}