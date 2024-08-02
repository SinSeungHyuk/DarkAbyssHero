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
    public float BonusValue { get; private set; } // �߰����� (���� ��)
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


    public void IncreaseBonusValue(float value)
    {
        float prevValue = Value;
        BonusValue += value; // ������ ���ʽ� ���ȿ� value �߰�
        OnValueChanged?.Invoke(this, Value, prevValue);
    }

    public void SetDefaultValueByPercent(float percent)
    {
        // percent % ��ŭ ����Ʈ ����� ���� (��ü Value�� ������)
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