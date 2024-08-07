
#region Effect Enums
using UnityEngine;

public enum EffectType
{
    None,
    Buff,
    Debuff
}

// Effect의 완료 시점이 언제인가?
public enum EffectRunningFinishOption
{
    // Effect가 설정된 적용 횟수만큼 적용된다면 완료되는 Option.
    // 단, 이 Option은 지속 시간(=Duration)이 끝나도 완료됨.
    // 타격을 입힌다던가, 치료를 해주는 Effect에 적합Option
    FinishWhenApplyCompleted,
    // 지속 시간이 끝나면 완료되는 Option.
    // Effect가 설정된 적용 횟수만큼 적용되도, 지속 시간이 남았다면 완료가 안됨.
    // 처음 한번 적용되고, 일정 시간동안 지속되는 Buff나 Debuff Effect에 적합한 Option.
    FinishWhenDurationEnded,
}
#endregion


#region Skill Enums
public enum SkillRunningFinishOption
{
    FinishWhenApplyCompleted, // ApplyCount가 0이 되면 스킬종료
    FinishWhenDurationEnded, // Duration이 모두 지나면 스킬종료
}

public enum SkillCustomActionType
{
    Cast,
    PrecedingAction,
    Action,
}

public enum SkillApplyType
{
    Instant, // 스킬 실행되자마자 이펙트 적용
    Animation // 스킬의 애니메이션에서 시점 결정
}

public enum AnimatorParameter
{
    isMagicAreaAttack,
    isUpHandCast,
    isClapCast,
    isStandingShoot,
}

// 스킬을 사용할때 메세지를 전달하기 위한 Enum
public enum SkillExecuteCommand
{
    Use, // 스킬 사용하는 순간 이 Use 커맨드를 보냄
    Find, // 플레이어 스테이트머신 내에서 사용할 스킬 찾았을때 (스킬의 스테이트에서 보내는게 아님)
    Ready, // 플레이어 스테이트머신 내에서 스킬의 대상을 찾았을때 (스킬의 스테이트에서 보내는게 아님)
    Cancel,
}
#endregion


#region Player State Enum
public enum PlayerStateCommand
{
    ToDefaultState,
    ToCastingSkillState,
    ToInSkillActionState,
    ToStunningState, // CC상태로 진입하라는 명령 (지금 당장은 사용x)
}

public enum EntityStateMessage { UsingSkill }
#endregion


#region Common
public enum GradeType
{
    Normal,
    Rare,
    Epic,
    Legend
}
#endregion



