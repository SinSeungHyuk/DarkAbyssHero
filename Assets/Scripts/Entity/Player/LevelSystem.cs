using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class LevelSystem : MonoBehaviour , ISaveData<LevelSaveData>
{
    //                          �������ġ,�ִ����ġ
    public event Action<LevelSystem, float,float> OnExpChanged;
    public event Action<LevelSystem, int> OnLevelChanged;

    private int level;
    private float exp;
    private float levelExp; // �������� �ʿ��� ����ġ


    public int Level
    {
        get => level;
        private set // �����ý��� �������� ���� ����
        {
            exp -= levelExp;
            levelExp *= Settings.expPerLevel; // ������ 1.04�� ������ (4% ����)

            level = value;

            OnExpChanged?.Invoke(this, exp,levelExp);
            OnLevelChanged?.Invoke(this, level);
        }
    }
    public float Exp { 
        get => exp;
        set {
            exp += value;

            if (exp >= levelExp)
            {
                Level++;
                return;
            }

            OnExpChanged?.Invoke(this, exp, levelExp);
        }
    }

    public Player Player { get; private set; }


    public void SetUp(Player owner)
    {
        Debug.Assert(owner != null, $"level::Setup - Owner�� Null�� �� �� �����ϴ�.");

        Player = owner;
        level = 1; // �ε��� �����Ͱ� ������ ������ �ε带 �ϹǷ� 1������ �¾�
        exp = 0;
        levelExp = Settings.startExp;
    }

    // �ѹ��� �뷮�� ����ġ�� ȹ�������� ȣ�� (������ ����)
    public void GetExpReward(int exp)
    {
        int levelUpCount = 0;
        float finalExp = this.exp + exp; // ���� ��������ġ + ���� ����ġ

        // ���� ���� ����ġ�� �������� �ʿ��� ����ġ���� ����������
        while (finalExp >= levelExp)
        {
            finalExp -= levelExp;
            levelExp *= Settings.expPerLevel;
            levelUpCount++;
        }

        level += levelUpCount;
        this.exp = finalExp;

        OnExpChanged?.Invoke(this, exp, levelExp);
        OnLevelChanged?.Invoke(this, level);
    }



    public LevelSaveData ToSaveData()
        => new LevelSaveData
        {
            level = level,
            exp = exp
        };

    public void FromSaveData(LevelSaveData saveData)
    {
        level = saveData.level;
        exp = saveData.exp;

        // �⺻���� : 1 , �ε��� ���� : 40
        // levelExp�� �� 39������ŭ �����ؾ��� (= expPerLevel�� 39����)
        levelExp *= (float)Math.Pow(Settings.expPerLevel, level - 1);

        OnExpChanged?.Invoke(this, exp, levelExp);
        OnLevelChanged?.Invoke(this, level);
    }
}

[Serializable]
public struct LevelSaveData
{
    public int level;
    public float exp;
}