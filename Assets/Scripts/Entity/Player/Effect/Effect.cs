using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect : IdentifiedObject
{
    // Duration 같은 변수는 Infinity(=0) 값이 들어가면 무한대를 의미
    private const int Infinity = 0;

    public event Action<Effect> OnEffectStart;
    public event Action<Effect, int, int> OnEffectApplied; // <남은횟수, 이전횟수>
    public event Action<Effect> OnEffectReleased;

    [SerializeField] private EffectType effectType;
    [SerializeField] private int maxLevel;
    [SerializeField] private EffectData[] effectDatas;

    private EffectData currentData;
    private int level; // 현재 레벨
    private bool isApplyTried; // 이펙트 적용을 시도했는지 여부 (적용주기를 위해 사용)
    private float currentDuration; // 현재 지난 지속시간
    private int currentApplyCount; // 현재 적용횟수
    private float currentApplyCycle; // 현재 지난 적용주기

    // 이펙트 타입이 Buff -> 스탯증가 이펙트 액션의 대상이 플레이어가 됨
    // Debuff -> 스탯증가(감소) 이펙트 액션의 대상이 Target Monster가 됨
    public EffectType EffectType => effectType;
    public IReadOnlyList<EffectData> EffectDatas => effectDatas;
    public int MaxLevel => maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value > 0 && value <= MaxLevel, $"Effect.Rank = {value} - value는 0보다 크고 MaxLevel보다 같거나 작아야합니다.");

            if (level == value)
                return;

            level = value;

            // 새로 세팅한 레벨과 '가장 가까운' 데이터 찾기
            EffectData newData = effectDatas.Last(x => x.level <= level);
            if (newData.level != currentData.level)
                currentData = newData;
        }
    }
    public bool IsMaxLevel => level == maxLevel;

    // 실제 Effect의 레벨과 EffectData의 레벨의 차이
    // 이 차이만큼 bonusValuePerLevel 곱해주기
    public int DataBonusLevel => Mathf.Max(level - currentData.level, 0);

    public float Duration => currentData.duration;
    // Duration = 0 : 무한지속되는 스킬 (ApplyCount에 의해 스킬이 종료됨)
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
    // applyCycle 값이 0 : Duration과 (ApplyCount-1)을 나누어서 일정한 간격을 구함
    // 만약 applyCycle가 0이 아니라면 그대로 데이터에 있는 사이클 값 사용
    public float ApplyCycle => Mathf.Approximately(currentData.applyCycle, 0f) && ApplyCycle > 1 
        ? (Duration / (ApplyCount - 1)) : currentData.applyCycle;
    public float CurrentApplyCycle
    {
        get => currentApplyCycle;
        set => currentApplyCycle = Mathf.Clamp(value, 0f, ApplyCycle);
    }

    private EffectAction effectAction => currentData.action;
    private CustomAction[] customActions => currentData.customActions;

    // Effect의 Owner는 Player가 아니라 Skill
    public Skill Skill { get; private set; }
    public Player Player { get; private set; }
    public Monster Target { get; private set; }

    private bool isDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool isApplyCompleted => CurrentApplyCount == ApplyCount;
    // Effect의 완료 여부
    // 지속 시간이 끝났거나, RunningFinishOption이 ApplyCompleted일 때, Apply 횟수가 최대 횟수라면 True
    public bool IsFinished => isDurationEnded ||
        (currentData.runningFinishOption == EffectRunningFinishOption.FinishWhenApplyCompleted && isApplyCompleted);
    // IsFinished와는 별개로 다른 외부에 의해 Effect가 종료되면 그 즉시 true되는 옵션
    public bool IsReleased { get; private set; }
    // 스킬 적용이 가능한지 여부 (적용횟수가 남았고 적용간격 시간이 충분히 지났으면)
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

    // MonoBehaviour가 없기 때문에 실제 Start할때 실행되진 않음
    public void Start()
    {
        Debug.Assert(!IsReleased, "Effect::Start - 이미 종료된 Effect");

        // 이펙트 액션의 Start 호출
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

        // 지속시간이 끝났으면 남은 적용횟수 한번에 적용시키기
        if (isDurationEnded)
        {
            for (int i = currentApplyCount; i < ApplyCount; i++)
                Apply();
        }
    }

    public void Apply()
    {
        Debug.Assert(!IsReleased, "Effect::Apply - 이미 종료된 Effect");

        if (effectAction == null) return;

        if (effectAction.Apply(this, Player, Target, level))
        {
            foreach (var customAction in customActions)
                customAction.Run(this);

            var prevApplyCount = CurrentApplyCount++;

            // 스킬 발동을 시도했지만 실패한적 있을때 currentApplyCycle = 0
            if (isApplyTried)
                currentApplyCycle = 0f;
            // currentApplyCycle는 Update를 통해 deltaTime이 더해지고 있음
            // 만약, ApplyCycle이 0.5라면 실제 currentApplyCycle는 0.512 같이 약간의 오차발생
            // 이 약간의 오차(0.12)를 currentApplyCycle에 넣어서 0.12에서부터 다시 0.5초까지
            // 즉, deltaTime을 더하면서 생기는 실제 ApplyCycle과의 약간의 오차를 메꾸는 과정
            else
                currentApplyCycle %= ApplyCycle;

            isApplyTried = false;

            OnEffectApplied?.Invoke(this, CurrentApplyCount, prevApplyCount);
        }
        // 스킬이 발동되지 못했지만 발동시도를 했을때 
        else
            isApplyTried = true;
    }

    public void Release()
    {
        Debug.Assert(!IsReleased, "Effect::Release - 이미 종료된 Effect입니다.");

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
