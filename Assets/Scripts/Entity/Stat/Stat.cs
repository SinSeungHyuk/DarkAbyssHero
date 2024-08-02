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
    public float BonusValue { get; private set; } // 추가스탯 (버프 등)
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


    public void IncreaseBonusValue(float value)
    {
        float prevValue = Value;
        BonusValue += value; // 현재의 보너스 스탯에 value 추가
        OnValueChanged?.Invoke(this, Value, prevValue);
    }

    public void SetDefaultValueByPercent(float percent)
    {
        // percent % 만큼 디폴트 밸류를 조절 (전체 Value가 조절됨)
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