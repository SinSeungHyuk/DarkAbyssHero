using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class Skill : IdentifiedObject, ISaveData<SkillSaveData>
{
    private const int Infinity = 0;

    // <스킬, 현재레벨, 이전레벨>
    public event Action<Skill, int, int> OnLevelChanged;
    // <스킬, 현재 스테이트, 이전 스테이트>
    public event Action<Skill, State<Skill>, State<Skill>> OnStateChanged;
    // <스킬, 현재 적용횟수>
    public event Action<Skill, int> OnSkillApplied;
    public event Action<Skill> OnSkillUsed;
    public event Action<Skill> OnSkillActivated;
    public event Action<Skill> OnSkillDeactivated;


    // 즉시적용 or 애니메이션 타이밍
    [SerializeField] private SkillApplyType applyType;
    // 스킬의 등급
    [SerializeField] private GradeType gradeType;

    [SerializeField, Min(1)] private int maxLevel = 1;
    [SerializeField, Min(1)] private int defaultLevel = 1;
    [SerializeField] private SkillData[] skillDatas;

    private SkillData currentData;

    private int level;

    private int currentApplyCount;
    private float currentCastTime;
    private float currentCooldown;
    private float currentDuration;

    private readonly Dictionary<SkillCustomActionType, CustomAction[]> customActionsByType = new();


    public Player Player { get; private set; }
    public StateMachine<Skill> StateMachine { get; private set; }

    public SkillApplyType ApplyType => applyType;

    public Grade SkillGrade { get; private set; }

    public IReadOnlyList<Effect> Effects { get; private set; } = new List<Effect>();


    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value >= 1 && value <= MaxLevel,
                 $"Skill.Rank = {value} - value는 1과 MaxLevel({MaxLevel}) 사이여야합니다.");


            if (level == value)
                return;

            int prevLevel = level;
            level = value;

            // 새로운 Level과 가장 가까운 Level Data를 찾아옴
            var newData = skillDatas.Last(x => x.level <= level);
            if (newData.level != currentData.level)
                ChangeData(newData);

            OnLevelChanged?.Invoke(this, level, prevLevel);
        }
    }
    public int DataBonusLevel => Mathf.Max(level - currentData.level, 0);
    public bool IsMaxLevel => level == maxLevel;

    private SkillAction SkillAction => currentData.action;

    public int CastAnimationParameter { get; private set; }
    public int ActionAnimationParameter { get; private set; }

    public float Cooldown => currentData.cooldown;
    public float CurrentCooldown
    {
        get => currentCooldown;
        set => currentData.cooldown = Mathf.Clamp(value, 0f, Cooldown);
    }
    public bool HasCooldown => Cooldown > 0f;
    public bool IsCooldownCompleted => Mathf.Approximately(Cooldown, 0f);

    public float Duration => currentData.duration;
    public float CurrentDuration
    {
        get => currentDuration;
        set => currentDuration = Mathf.Clamp(value, 0f, Duration);
    }

    public SkillRunningFinishOption RunningFinishIption => currentData.runningFinishOption;
    public int ApplyCount => currentData.applyCount;
    public int CurrentApplyCount
    {
        get => currentApplyCount;
        set
        {
            if (currentApplyCount == value)
                return;

            currentApplyCount = Mathf.Clamp(value, 0, ApplyCount);
        }
    }
    // applyCycle 값이 0 : Duration과 (ApplyCount-1)을 나누어서 일정한 간격을 구함
    // 만약 applyCycle가 0이 아니라면 그대로 데이터에 있는 사이클 값 사용
    public float ApplyCycle => Mathf.Approximately(currentData.applyCycle, 0f) && ApplyCount > 1 ?
        Duration / (ApplyCount - 1) : currentData.applyCycle;
    public float CurrentApplyCycle { get; set; }

    public bool IsUseCast => currentData.isUseCast;
    public float CastTime => currentData.castTime;
    public float CurrentCastTime
    {
        get => currentCastTime;
        set => currentCastTime = Mathf.Clamp(value, 0f, CastTime);
    }
    public bool IsCastCompleted => Mathf.Approximately(CastTime, CurrentCastTime);

    public bool IsActivated { get; private set; }
    public bool IsReady => StateMachine.IsInState<ReadyState>();
    // 발동 횟수가 남았고, ApplyCycle만큼 시간이 지났으면 true를 return
    public bool IsApplicable => (CurrentApplyCount < ApplyCount) && (CurrentApplyCycle >= ApplyCycle);

    // 스킬의 타겟 (플레이어 자신도 포함)
    // 만약 자신에게 버프'만' 주는 스킬이라면 Target이 Player 하나만 존재
    // 적에게 데미지를 주고 자신에게 버프를 준다면, Target은 몬스터로 설정하고 이펙트 두개 넣기
    public Entity Target
    {
        get => Target;
        set
        {
            if (value == null) return;

            Target = value;
            TargetPosition = Target.gameObject.transform.position;
        }
    }
    public Vector3 TargetPosition { get; private set; }

    private bool IsDurationEnded => Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => CurrentApplyCount == ApplyCount;
    // 스킬이 끝났는지 판단 = 스킬의 종료시점이 Duration인지, ApplyCount인지에 따라 달라짐
    public bool IsFinished => currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded ?
    IsDurationEnded : IsApplyCompleted;



    public void OnDestroy()
    {
        // Effects 리스트의 원소들은 Clone() 함수로 복사된 객체들
        foreach (var effect in Effects)
            Destroy(effect);
    }

    public void SetUp(Player owner, int level)
    {
        Debug.Assert(owner != null, $"Skill::Setup - Owner는 Null이 될 수 없습니다.");
        Debug.Assert(level >= 1 && level <= maxLevel, $"Skill::Setup - {level}이 1보다 작거나 {maxLevel}보다 큽니다.");
        Debug.Assert(Player == null, $"Skill::Setup - 이미 Setup하였습니다.");

        Player = owner;
        Level = level;

        SetAnimatorParameter();
        SkillGrade = new Grade(gradeType); // gradeType으로 스킬의 등급 생성

        StateMachine = new InstantSkillStateMachine();

        StateMachine.SetUp(this);
        StateMachine.OnStateChanged += (_, newState, prevState, layer)
            => OnStateChanged?.Invoke(this, newState, prevState);
    }

    public void SetUp(Player owner)
        => SetUp(owner, defaultLevel);

    // SkillSystem Update -> Skill Update -> StateMachine Update -> State Update
    public void Update() => StateMachine.Update();

    private void SetAnimatorParameter()
    {
        switch (currentData.castAnimatorParameter)
        {
            case AnimatorParameter.isMagicAreaAttack:
                CastAnimationParameter = Settings.isMagicAreaAttack;
                break;
            case AnimatorParameter.isUpHandCast:
                CastAnimationParameter = Settings.isUpHandCast;
                break;
            case AnimatorParameter.isClapCast:
                CastAnimationParameter = Settings.isClapCast;
                break;
            case AnimatorParameter.isStandingShoot:
                CastAnimationParameter = Settings.isStandingShoot;
                break;
        }
        switch (currentData.actionAnimatorParameter)
        {
            case AnimatorParameter.isMagicAreaAttack:
                ActionAnimationParameter = Settings.isMagicAreaAttack;
                break;
            case AnimatorParameter.isUpHandCast:
                ActionAnimationParameter = Settings.isUpHandCast;
                break;
            case AnimatorParameter.isClapCast:
                ActionAnimationParameter = Settings.isClapCast;
                break;
            case AnimatorParameter.isStandingShoot:
                ActionAnimationParameter = Settings.isStandingShoot;
                break;
        }
    }

    // 스킬의 데이터를 레벨에 맞는 데이터로 교체
    private void ChangeData(SkillData newData)
    {
        // 기존의 Effect들 파괴
        foreach (var effect in Effects)
            Destroy(effect);

        currentData = newData;

        Effects = currentData.effectSelectors.Select(x => x.CreateEffect(this)).ToArray();
        // Skill의 현재 Level이 data의 Level보다 크면, 둘의 Level 차를 Effect의 Bonus Level 줌.
        // 만약 Skill이 2 Level이고, data가 1 level이라면, effect들은 2-1해서 1의 Bonus Level을 받게 됨.
        if (level > currentData.level)
            UpdateCurrentEffectLevels();

        UpdateCustomActions();
    }
    private void UpdateCurrentEffectLevels()
    {
        int bonusLevel = DataBonusLevel;
        foreach (var effect in Effects)
            effect.Level = Mathf.Min(effect.Level + bonusLevel, effect.MaxLevel);
    }
    private void UpdateCustomActions()
    {
        customActionsByType[SkillCustomActionType.Cast] = currentData.customActionsOnCast;
        customActionsByType[SkillCustomActionType.Action] = currentData.customActionsOnAction;
    }

    public void ResetProperties()
    {
        CurrentCastTime = 0f;
        CurrentCooldown = 0f;
        CurrentDuration = 0f;
        CurrentApplyCycle = 0f;
        CurrentApplyCount = 0;
    }

    #region Use & Activate
    public bool Use()
    {
        Debug.Assert(IsReady, "Skill::Use - 사용 조건을 만족하지 못했습니다.");

        // 이 스킬의 스테이트머신에 Use 커맨드를 보내서 스킬사용 시작하기
        bool isUsed = StateMachine.ExecuteCommand(SkillExecuteCommand.Use);
        if (isUsed)
            OnSkillUsed?.Invoke(this);

        return isUsed;
    }

    public bool Cancel()
    {
        bool isCanceled = StateMachine.ExecuteCommand(SkillExecuteCommand.Cancel);

        return isCanceled;
    }

    public void Activate()
    {
        Debug.Assert(!IsActivated, "Skill::Activate - 이미 활성화되어 있습니다.");

        IsActivated = true;
        OnSkillActivated?.Invoke(this);
    }

    public void Deactivate()
    {
        Debug.Assert(IsActivated, "Skill::Activate - Skill이 활성화되어있지 않습니다.");

        IsActivated = false;
        OnSkillDeactivated?.Invoke(this);
    }
    #endregion

    #region CustomAction 
    public void StartCustomActions(SkillCustomActionType type)
    {
        foreach (var customAction in customActionsByType[type])
            customAction.Start(this);
    }
    public void RunCustomActions(SkillCustomActionType type)
    {
        foreach (var customAction in customActionsByType[type])
            customAction.Run(this);
    }
    public void ReleaseCustomActions(SkillCustomActionType type)
    {
        foreach (var customAction in customActionsByType[type])
            customAction.Release(this);
    }
    #endregion

    #region SkillAction Start / Apply / Release
    public void StartSkillAction()
    {
        StartCustomActions(SkillCustomActionType.Action);
        SkillAction.Start(this);
    }

    public void ReleaseSkillAction()
    {
        ReleaseCustomActions(SkillCustomActionType.Action);
        SkillAction.Release(this);
    }

    public void Apply()
    {
        Debug.Assert(IsApplicable, "Skill - Apply를 할 수 없습니다.");

        RunCustomActions(SkillCustomActionType.Action);

        SkillAction.Apply(this);

        // Duration과의 오차 값을 남기기 위해 ApplyCycle로 나눈 나머지로 값을 설정함
        // Ex. Duration = 1.001, CurrentApplyCycle = 1.001
        //     => Duration = 1.001, CurrentApplyCycle = 0.001
        CurrentApplyCycle %= ApplyCycle;
        CurrentApplyCount++;

        OnSkillApplied?.Invoke(this, CurrentApplyCount);
    }
    #endregion

    // 이 스킬의 스테이트머신이 <T> 스테이트인지 확인 (wrapping)
    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();

    public override object Clone()
    {
        var clone = Instantiate(this);

        if (Player != null)
            clone.SetUp(Player, level);

        return clone;
    }



    public SkillSaveData ToSaveData()
    => new SkillSaveData
    {
        id = ID,
        level = level
    };

    public void FromSaveData(SkillSaveData saveData)
    {
        // 저장된 ID는 SkillSystem에서 사용
        level = saveData.level;
    }
}


[Serializable]
public struct SkillSaveData
{
    public int id;
    public int level;
}