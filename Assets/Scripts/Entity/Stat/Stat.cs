using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Stat : IdentifiedObject, ISaveData<StatSaveData>
{
    //             Stat ����, ���� ��, ���� ��
    public event Action<Stat, float, float> OnValueChanged;
    public event Action<Stat, float> OnLevelChanged;

    [SerializeField] private int requiredLevel; // ���ȼ��� �䱸����

    [SerializeField] private bool isUseMaxValue; // �ִ� ������ �ִ���
    [SerializeField] private float maxValue; // �ִ��ġ
    [SerializeField] private float defaultValue; // �⺻��ġ
    [SerializeField] private float valuePerLevel; // ������ ��������
    [SerializeField] private int goldPerLevel; // ������ �������
    [SerializeField] private int defaultGold; // �⺻ ���ۺ��
    
    private int level = 1; // ��� ������ 1���� ����
    // key : ���ʽ� ������ �� ���, value : ���� ���ʽ� ���� ��ġ
    private Dictionary<object, float> bonusValuesByKey = new();


    public float MaxValue
    {
        get => maxValue;
        set
        {
            if (value != maxValue && isUseMaxValue)
            {
                float prevValue = Value;

                maxValue = value;
                defaultValue += value;

                OnValueChanged?.Invoke(this, Value, prevValue);
            }
        }
    }
    public float DefaultValue
    {
        get => defaultValue;
        set
        {
            if (value != defaultValue)
            {
                float prevValue = Value;
                
                if (isUseMaxValue && value >= maxValue)               
                    value = maxValue;
                
                defaultValue = value;
                OnValueChanged?.Invoke(this, Value, prevValue);
            }
        }
    }
    public float BonusValue { get; private set; } // bonusValuesByKey ��� ����
    public float Value => DefaultValue + BonusValue; // ������ ����� �� ����
    public int Level
    {
        get => level;
        set
        {
            if (value <= level) return;

            float prevValue = Value;
            if (isUseMaxValue) maxValue += valuePerLevel;
            defaultValue += valuePerLevel;
            level = value;
            OnLevelChanged?.Invoke(this, level);
        }
    }

    public int RequiredLevel => requiredLevel;
    public float ValuePerLevel => valuePerLevel;
    public int GoldPerLevel => goldPerLevel;
    public int DefaultGold => defaultGold;
    public int CurrentGold => defaultGold + (goldPerLevel * (level-1));


    public void IncreaseBonusValue(object key, float value)
    {
        // �̹� �ش� key���� ���� ���ʽ� ������ �����ϸ� ���� ���ʽ� �����
        if (bonusValuesByKey.TryGetValue(key, out float prevBonus))
            BonusValue -= prevBonus;

        float prevValue = Value;
        bonusValuesByKey[key] = value;
        BonusValue += value; // ������ ���ʽ� ���ȿ� value �߰�

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
        // ��ü Value�� percent% ��ŭ�� ��ġ�� ��� �� key���� ���� ���ʽ������ �ֱ�
        // ������� ������ų�� ���ݷ� 10% ���� -> ��ü ���ݷ� Value�� 10% ���
        // ���� �ش� ��ų�� key������ ����� 10% ��ġ�� ���ʽ������ ���ϱ�

        float bonusValue = Value * percent;
        IncreaseBonusValue(key, bonusValue);
    }


    public StatSaveData ToSaveData()
        => new StatSaveData
        {
            id = ID,
            maxValue = maxValue,
            defaultValue = defaultValue,
            level = level
        };

    public void FromSaveData(StatSaveData saveData)
    {
        // ����� ID�� Stats���� ���
        maxValue = saveData.maxValue;
        defaultValue = saveData.defaultValue;
        level = saveData.level;
    }
}

[System.Serializable]
public struct StatSaveData
{
    public int id;
    public float maxValue;
    public float defaultValue;
    public int level; // ������ ���巹��
}