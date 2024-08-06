using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class Skill : IdentifiedObject, ISaveData<SkillSaveData>
{
    private const int Infinity = 0;

    // <��ų, ���緹��, ��������>
    public event Action<Skill, int, int> OnLevelChanged;
    // <��ų, ���� ������Ʈ, ���� ������Ʈ>
    public event Action<Skill, State<Skill>, State<Skill>> OnStateChanged;
    // <��ų, ���� ����Ƚ��>
    public event Action<Skill, int> OnSkillApplied;
    public event Action<Skill> OnSkillUsed;
    public event Action<Skill> OnSkillActivated;
    public event Action<Skill> OnSkillDeactivated;


    // ������� or �ִϸ��̼� Ÿ�̹�
    [SerializeField] private SkillApplyType applyType;
    // ��ų�� ���
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
                 $"Skill.Rank = {value} - value�� 1�� MaxLevel({MaxLevel}) ���̿����մϴ�.");


            if (level == value)
                return;

            int prevLevel = level;
            level = value;

            // ���ο� Level�� ���� ����� Level Data�� ã�ƿ�
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
    // applyCycle ���� 0 : Duration�� (ApplyCount-1)�� ����� ������ ������ ����
    // ���� applyCycle�� 0�� �ƴ϶�� �״�� �����Ϳ� �ִ� ����Ŭ �� ���
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
    // �ߵ� Ƚ���� ���Ұ�, ApplyCycle��ŭ �ð��� �������� true�� return
    public bool IsApplicable => (CurrentApplyCount < ApplyCount) && (CurrentApplyCycle >= ApplyCycle);

    // ��ų�� Ÿ�� (�÷��̾� �ڽŵ� ����)
    // ���� �ڽſ��� ����'��' �ִ� ��ų�̶�� Target�� Player �ϳ��� ����
    // ������ �������� �ְ� �ڽſ��� ������ �شٸ�, Target�� ���ͷ� �����ϰ� ����Ʈ �ΰ� �ֱ�
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
    // ��ų�� �������� �Ǵ� = ��ų�� ��������� Duration����, ApplyCount������ ���� �޶���
    public bool IsFinished => currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded ?
    IsDurationEnded : IsApplyCompleted;



    public void OnDestroy()
    {
        // Effects ����Ʈ�� ���ҵ��� Clone() �Լ��� ����� ��ü��
        foreach (var effect in Effects)
            Destroy(effect);
    }

    public void SetUp(Player owner, int level)
    {
        Debug.Assert(owner != null, $"Skill::Setup - Owner�� Null�� �� �� �����ϴ�.");
        Debug.Assert(level >= 1 && level <= maxLevel, $"Skill::Setup - {level}�� 1���� �۰ų� {maxLevel}���� Ů�ϴ�.");
        Debug.Assert(Player == null, $"Skill::Setup - �̹� Setup�Ͽ����ϴ�.");

        Player = owner;
        Level = level;

        SetAnimatorParameter();
        SkillGrade = new Grade(gradeType); // gradeType���� ��ų�� ��� ����

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

    // ��ų�� �����͸� ������ �´� �����ͷ� ��ü
    private void ChangeData(SkillData newData)
    {
        // ������ Effect�� �ı�
        foreach (var effect in Effects)
            Destroy(effect);

        currentData = newData;

        Effects = currentData.effectSelectors.Select(x => x.CreateEffect(this)).ToArray();
        // Skill�� ���� Level�� data�� Level���� ũ��, ���� Level ���� Effect�� Bonus Level ��.
        // ���� Skill�� 2 Level�̰�, data�� 1 level�̶��, effect���� 2-1�ؼ� 1�� Bonus Level�� �ް� ��.
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
        Debug.Assert(IsReady, "Skill::Use - ��� ������ �������� ���߽��ϴ�.");

        // �� ��ų�� ������Ʈ�ӽſ� Use Ŀ�ǵ带 ������ ��ų��� �����ϱ�
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
        Debug.Assert(!IsActivated, "Skill::Activate - �̹� Ȱ��ȭ�Ǿ� �ֽ��ϴ�.");

        IsActivated = true;
        OnSkillActivated?.Invoke(this);
    }

    public void Deactivate()
    {
        Debug.Assert(IsActivated, "Skill::Activate - Skill�� Ȱ��ȭ�Ǿ����� �ʽ��ϴ�.");

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
        Debug.Assert(IsApplicable, "Skill - Apply�� �� �� �����ϴ�.");

        RunCustomActions(SkillCustomActionType.Action);

        SkillAction.Apply(this);

        // Duration���� ���� ���� ����� ���� ApplyCycle�� ���� �������� ���� ������
        // Ex. Duration = 1.001, CurrentApplyCycle = 1.001
        //     => Duration = 1.001, CurrentApplyCycle = 0.001
        CurrentApplyCycle %= ApplyCycle;
        CurrentApplyCount++;

        OnSkillApplied?.Invoke(this, CurrentApplyCount);
    }
    #endregion

    // �� ��ų�� ������Ʈ�ӽ��� <T> ������Ʈ���� Ȯ�� (wrapping)
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
        // ����� ID�� SkillSystem���� ���
        level = saveData.level;
    }
}


[Serializable]
public struct SkillSaveData
{
    public int id;
    public int level;
}