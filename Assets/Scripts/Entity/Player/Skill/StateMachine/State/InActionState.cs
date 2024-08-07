using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InActionState : SkillState
{
    private bool isInstantApplyType;

    protected override void Awake()
    {
        isInstantApplyType = TOwner.ApplyType == SkillApplyType.Instant;
    }

    public override void Enter()
    {
        Debug.Log("InActionState Enter!!");

        if (!TOwner.IsActivated)
            TOwner.Activate();

        // 처음 스킬의 액션상태에 진입하면서 SkillAction의 Start 실행
        // 투사체 발사 스킬액션 등은 Start 구현이 안되어있음
        TOwner.StartSkillAction();

        // 처음 스킬 액션상태에 진입하면서 Apply까지 한 번 해주기
        Apply();
    }

    public override void Update()
    {
        TOwner.CurrentDuration += Time.deltaTime;
        TOwner.CurrentApplyCycle += Time.deltaTime;

        // 스킬이 적용 가능하다면 Apply로 적용 실행
        if (TOwner.IsApplicable)
            Apply();
    }

    public override void Exit()
    {
        Debug.Log("InActionState Exit!!");
        TOwner.ReleaseSkillAction();
    }

    private void Apply()
    {
        // 플레이어에게 이 스킬을 사용했으니 ToInSkillActionState로 전환하라는 명령 보내기
        TrySendCommandToPlayer(TOwner, PlayerStateCommand.ToInSkillActionState, TOwner.ActionAnimationParameter);

        // Skill의 Apply 함수를 이 스킬의 스테이트에서 호출하거나 혹은
        // 애니메이션이 재생되면서 자동으로 애니메이션의 특정 타이밍에서 호출하거나
        if (isInstantApplyType)
            TOwner.Apply();
        else 
            TOwner.CurrentApplyCount++;
    }
}