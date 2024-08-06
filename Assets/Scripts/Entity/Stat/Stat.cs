using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stat : IdentifiedObject, ISaveData<StatSaveData>
{
    //             Stat 종류, 변경 후, 변경 전
    public event Action<Stat, float, float> OnValueChanged;

    [SerializeField] private float defaultValue; // 기본수치
    [SerializeField] private float valuePerLevel; // 레벨당 증가스탯
    [SerializeField] private float goldPerLevel; // 레벨당 증가비용
    [SerializeField] private float defaultGold; // 기본 업글비용
    
    private int level = 1; // 모든 스탯은 1레벨 시작
    // key : 보너스 스탯을 준 대상, value : 얻은 보너스 스탯 수치
    private Dictionary<object, float> bonusValuesByKey = new();


    public float DefaultValue
    {
        get => defaultValue;
        set
        {
            if (value != defaultValue)
            {
                float prevValue = Value;
                defaultValue = value;
                OnValueChanged?.Invoke(this, Value, prevValue);
            }
        }
    }
    public float BonusValue { get; private set; } // bonusValuesByKey 밸류 총합
    public float Value => defaultValue + BonusValue; // 실제로 사용할 총 스탯
    public int Level
    {
        get => level;
        set
        {
            if (value <= level) return;

            int addLevel = value - level;
            float prevValue = Value;
            defaultValue += (valuePerLevel * addLevel);
            level = value;
            OnValueChanged?.Invoke(this, Value, prevValue);
        }
    }

    public float ValuePerLevel => valuePerLevel;
    public float GoldPerLevel => goldPerLevel;
    public float DefaultGold => defaultGold;


    public void IncreaseBonusValue(object key, float value)
    {
        // 이미 해당 key한테 받은 보너스 스탯이 존재하면 기존 보너스 지우기
        if (bonusValuesByKey.TryGetValue(key, out float prevBonus))
            BonusValue -= prevBonus;

        float prevValue = Value;
        bonusValuesByKey[key] = value;
        BonusValue += value; // 현재의 보너스 스탯에 value 추가

        OnValueChanged?.Invoke(this, Value, prevValue);
    }

    public bool RemoveBonusValue(object key)
    {
        if (bonusValuesByKey.TryGetValue(key, out float bonusValue))
        {
            float prevValue = Value;
            BonusValue -= bonusValue;
            bonusValuesByKey.Remove(key);

            OnValueChanged?.Invoke(this, Value, prevValue);
            return true;
        }
        return false;
    }

    public void SetValueByPercent(object key, float percent)
    {
        // 전체 Value의 percent% 만큼의 수치를 계산 후 key값을 통해 보너스밸류에 넣기
        // 예를들어 버프스킬로 공격력 10% 버프 -> 전체 공격력 Value의 10% 계산
        // 이후 해당 스킬을 key값으로 계산한 10% 수치를 보너스밸류에 더하기
    }


    public StatSaveData ToSaveData()
        => new StatSaveData
        {
            id = ID,
            defaultValue = defaultValue,
            level = level
        };

    public void FromSaveData(StatSaveData saveData)
    {
        // 저장된 ID는 Stats에서 사용
        defaultValue = saveData.defaultValue;
        level = saveData.level;
    }
}

[System.Serializable]
public struct StatSaveData
{
    public int id;
    public float defaultValue;
    public int level; // 스탯의 성장레벨
}