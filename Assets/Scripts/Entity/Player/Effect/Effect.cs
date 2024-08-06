using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect : IdentifiedObject
{
    // Duration ���� ������ Infinity(=0) ���� ���� ���Ѵ븦 �ǹ�
    private const int Infinity = 0;

    public event Action<Effect> OnEffectStart;
    public event Action<Effect, int, int> OnEffectApplied; // <����Ƚ��, ����Ƚ��>
    public event Action<Effect> OnEffectReleased;

    [SerializeField] private EffectType effectType;
    [SerializeField] private int maxLevel;
    [SerializeField] private EffectData[] effectDatas;

    private EffectData currentData;
    private int level; // ���� ����
    private bool isApplyTried; // ����Ʈ ������ �õ��ߴ��� ���� (�����ֱ⸦ ���� ���)
    private float currentDuration; // ���� ���� ���ӽð�
    private int currentApplyCount; // ���� ����Ƚ��
    private float currentApplyCycle; // ���� ���� �����ֱ�

    // ����Ʈ Ÿ���� Buff -> �������� ����Ʈ �׼��� ����� �÷��̾ ��
    // Debuff -> ��������(����) ����Ʈ �׼��� ����� Target Monster�� ��
    public EffectType EffectType => effectType;
    public IReadOnlyList<EffectData> EffectDatas => effectDatas;
    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value > 0 && value <= MaxLevel, $"Effect.Rank = {value} - value�� 0���� ũ�� MaxLevel���� ���ų� �۾ƾ��մϴ�.");

            if (level == value)
                return;

            level = value;

            // ���� ������ ������ '���� �����' ������ ã��
            EffectData newData = effectDatas.Last(x => x.level <= level);
            if (newData.level != currentData.level)
                currentData = newData;
        }
    }
    public bool IsMaxLevel => level == maxLevel;

    // ���� Effect�� ������ EffectData�� ������ ����
    // �� ���̸�ŭ bonusValuePerLevel �����ֱ�
    public int DataBonusLevel => Mathf.Max(level - currentData.level, 0);

    public float Duration => currentData.duration;
    // Duration = 0 : �������ӵǴ� ��ų (ApplyCount�� ���� ��ų�� �����)
    public bool IsTimeless => Mathf.Approximately(Duration, Infinity); 
    public float CurrentDuration
    {
        get => currentDuration;
        set => currentDuration = Mathf.Clamp(value, 0f, Duration);
    }
    public float RemainDuration => Mathf.Max(0f, Duration - currentDuration);

    public int ApplyCount => currentData.applyCount;
    public int CurrentApplyCount
    {
        get => currentApplyCount;
        set => currentApplyCount = Mathf.Clamp(value, 0, ApplyCount);
    }
    // applyCycle ���� 0 : Duration�� (ApplyCount-1)�� ����� ������ ������ ����
    // ���� applyCycle�� 0�� �ƴ϶�� �״�� �����Ϳ� �ִ� ����Ŭ �� ���
    public float ApplyCycle => Mathf.Approximately(currentData.applyCycle, 0f) && ApplyCycle > 1 
        ? (Duration / (ApplyCount - 1)) : currentData.applyCycle;
    public float CurrentApplyCycle
    {
        get => currentApplyCycle;
        set => currentApplyCycle = Mathf.Clamp(value, 0f, ApplyCycle);
    }

    private EffectAction effectAction => currentData.action;
    private CustomAction[] customActions => currentData.customActions;

    // Effect�� Owner�� Player�� �ƴ϶� Skill
    public Skill Skill { get; private set; }
    public Player Player { get; private set; }
    public Monster Target { get; private set; }

    private bool isDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool isApplyCompleted => CurrentApplyCount == ApplyCount;
    // Effect�� �Ϸ� ����
    // ���� �ð��� �����ų�, RunningFinishOption�� ApplyCompleted�� ��, Apply Ƚ���� �ִ� Ƚ����� True
    public bool IsFinished => isDurationEnded ||
        (currentData.runningFinishOption == EffectRunningFinishOption.FinishWhenApplyCompleted && isApplyCompleted);
    // IsFinished�ʹ� ������ �ٸ� �ܺο� ���� Effect�� ����Ǹ� �� ��� true�Ǵ� �ɼ�
    public bool IsReleased { get; private set; }
    // ��ų ������ �������� ���� (����Ƚ���� ���Ұ� ���밣�� �ð��� ����� ��������)
    public bool IsApplicable => effectAction != null &&
    (CurrentApplyCount < ApplyCount) && CurrentApplyCycle >= ApplyCycle;


    public void SetUp(Skill owner, Player user, int level)
    {
        Skill = owner;
        Player = user;
        Level = level;
        CurrentApplyCycle = ApplyCycle;
    }

    public void SetTarget(Monster target) => Target = target;

    // MonoBehaviour�� ���� ������ ���� Start�Ҷ� ������� ����
    public void Start()
    {
        Debug.Assert(!IsReleased, "Effect::Start - �̹� ����� Effect");

        // ����Ʈ �׼��� Start ȣ��
        effectAction?.Start(this, Player, Target, Level);

        foreach (var customAction in customActions)
            customAction?.Start(this);

        OnEffectStart?.Invoke(this);
    }

    public void Update()
    {
        CurrentDuration += Time.deltaTime;
        currentApplyCycle += Time.deltaTime;

        if (IsApplicable)
            Apply();

        // ���ӽð��� �������� ���� ����Ƚ�� �ѹ��� �����Ű��
        if (isDurationEnded)
        {
            for (int i = currentApplyCount; i < ApplyCount; i++)
                Apply();
        }
    }

    public void Apply()
    {
        Debug.Assert(!IsReleased, "Effect::Apply - �̹� ����� Effect");

        if (effectAction == null) return;

        if (effectAction.Apply(this, Player, Target, level))
        {
            foreach (var customAction in customActions)
                customAction.Run(this);

            var prevApplyCount = CurrentApplyCount++;

            // ��ų �ߵ��� �õ������� �������� ������ currentApplyCycle = 0
            if (isApplyTried)
                currentApplyCycle = 0f;
            // currentApplyCycle�� Update�� ���� deltaTime�� �������� ����
            // ����, ApplyCycle�� 0.5��� ���� currentApplyCycle�� 0.512 ���� �ణ�� �����߻�
            // �� �ణ�� ����(0.12)�� currentApplyCycle�� �־ 0.12�������� �ٽ� 0.5�ʱ���
            // ��, deltaTime�� ���ϸ鼭 ����� ���� ApplyCycle���� �ణ�� ������ �޲ٴ� ����
            else
                currentApplyCycle %= ApplyCycle;

            isApplyTried = false;

            OnEffectApplied?.Invoke(this, CurrentApplyCount, prevApplyCount);
        }
        // ��ų�� �ߵ����� �������� �ߵ��õ��� ������ 
        else
            isApplyTried = true;
    }

    public void Release()
    {
        Debug.Assert(!IsReleased, "Effect::Release - �̹� ����� Effect�Դϴ�.");

        effectAction?.Release(this, Player, Target, level);

        foreach (var customAction in customActions)
            customAction.Release(this);

        IsReleased = true;

        OnEffectReleased?.Invoke(this);
    }

    public EffectData GetData(int level) => effectDatas[level - 1];


    public override object Clone()
    {
        Effect clone = Instantiate(this);

        if (Skill != null)
            clone.SetUp(Skill, Player, Level);

        return clone;
    }
}
