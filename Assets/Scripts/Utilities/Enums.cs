
#region Effect Enums
using UnityEngine;

public enum EffectType
{
    None,
    Buff,
    Debuff
}

// Effect�� �Ϸ� ������ �����ΰ�?
public enum EffectRunningFinishOption
{
    // Effect�� ������ ���� Ƚ����ŭ ����ȴٸ� �Ϸ�Ǵ� Option.
    // ��, �� Option�� ���� �ð�(=Duration)�� ������ �Ϸ��.
    // Ÿ���� �����ٴ���, ġ�Ḧ ���ִ� Effect�� ����Option
    FinishWhenApplyCompleted,
    // ���� �ð��� ������ �Ϸ�Ǵ� Option.
    // Effect�� ������ ���� Ƚ����ŭ ����ǵ�, ���� �ð��� ���Ҵٸ� �Ϸᰡ �ȵ�.
    // ó�� �ѹ� ����ǰ�, ���� �ð����� ���ӵǴ� Buff�� Debuff Effect�� ������ Option.
    FinishWhenDurationEnded,
}
#endregion


#region Skill Enums
public enum SkillRunningFinishOption
{
    FinishWhenApplyCompleted, // ApplyCount�� 0�� �Ǹ� ��ų����
    FinishWhenDurationEnded, // Duration�� ��� ������ ��ų����
}

public enum SkillCustomActionType
{
    Cast,
    PrecedingAction,
    Action,
}

public enum SkillApplyType
{
    Instant, // ��ų ������ڸ��� ����Ʈ ����
    Animation // ��ų�� �ִϸ��̼ǿ��� ���� ����
}

public enum AnimatorParameter
{
    isMagicAreaAttack,
    isUpHandCast,
    isClapCast,
    isStandingShoot,
}

// ��ų�� ����Ҷ� �޼����� �����ϱ� ���� Enum
public enum SkillExecuteCommand
{
    Use, // ��ų ����ϴ� ���� �� Use Ŀ�ǵ带 ����
    Find, // �÷��̾� ������Ʈ�ӽ� ������ ����� ��ų ã������ (��ų�� ������Ʈ���� �����°� �ƴ�)
    Ready, // �÷��̾� ������Ʈ�ӽ� ������ ��ų�� ����� ã������ (��ų�� ������Ʈ���� �����°� �ƴ�)
    Cancel,
}
#endregion


#region Player State Enum
public enum PlayerStateCommand
{
    ToDefaultState,
    ToCastingSkillState,
    ToInSkillActionState,
    ToStunningState, // CC���·� �����϶�� ��� (���� ������ ���x)
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



