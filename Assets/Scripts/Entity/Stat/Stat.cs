using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stat : IdentifiedObject, ISaveData<StatSaveData>
{
    //             Stat ����, ���� ��, ���� ��
    public event Action<Stat, float, float> OnValueChanged;

    [SerializeField] private float defaultValue; // �⺻��ġ
    [SerializeField] private float valuePerLevel; // ������ ��������
    [SerializeField] private float goldPerLevel; // ������ �������
    [SerializeField] private float defaultGold; // �⺻ ���ۺ��
    
    private int level = 1; // ��� ������ 1���� ����
    // key : ���ʽ� ������ �� ���, value : ���� ���ʽ� ���� ��ġ
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
    public float BonusValue { get; private set; } // bonusValuesByKey ��� ����
    public float Value => defaultValue + BonusValue; // ������ ����� �� ����
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
        // ����� ID�� Stats���� ���
        defaultValue = saveData.defaultValue;
        level = saveData.level;
    }
}

[System.Serializable]
public struct StatSaveData
{
    public int id;
    public float defaultValue;
    public int level; // ������ ���巹��
}