using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InActionState : SkillState
{
    private bool isInstantApplyType;

    protected override void Awake()
    {
        // 즉시적용인지, 애니메이션 적용인지 여부
        isInstantApplyType = TOwner.ApplyType == SkillApplyType.Instant;
    }

    public override void Enter()
    {
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
        // 현재 사용한 스킬의 Duration, ApplyCycle 더해주기
        TOwner.CurrentDuration += Time.deltaTime;
        TOwner.CurrentApplyCycle += Time.deltaTime;

        // 스킬이 적용 가능하다면 Apply로 적용 실행 (IsApplicable = 적용횟수,간격 비교)
        if (TOwner.IsApplicable)
            Apply();
    }

    public override void Exit()
    {
        TOwner.ReleaseSkillAction();

        // 플레이에게 스킬이 끝났다는 메세지 보내기 
        // TrySendCommandToPlayer 가 아니라 SendMessage 이므로 현재 실행중인 스테이트에게 전달됨
        TOwner.Player.StateMachine.SendMessage(EntityStateMessage.FinishSkill);
    }

    private void Apply()
    {
        // 플레이어에게 이 스킬을 사용했으니 ToInSkillActionState로 전이하라는 명령 보내기
        TrySendCommandToPlayer(TOwner, PlayerStateCommand.ToInSkillActionState, TOwner.ActionAnimationParameter);

        // Skill의 Apply 함수를 이 스킬의 스테이트에서 호출하거나 혹은
        // 애니메이션이 재생되면서 자동으로 애니메이션의 특정 타이밍에서 호출하거나
        if (isInstantApplyType)
            TOwner.Apply(); // Skill의 Apply 함수 호출
    }
}
