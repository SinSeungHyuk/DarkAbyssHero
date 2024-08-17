using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTargetState : State<Player>
{ 
    public override void Update()
    {
        // 대상과 거리가 충분히 가까워졌을때 스킬 사용
        if (TOwner.Movement.IsStop)
            TOwner.SkillSystem.ReserveSkill.Use();
    }
}