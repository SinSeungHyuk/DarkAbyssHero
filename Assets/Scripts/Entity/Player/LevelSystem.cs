using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class LevelSystem : MonoBehaviour , ISaveData<LevelSaveData>
{
    //                          현재경험치,최대경험치
    public event Action<LevelSystem, float,float> OnExpChanged;
    public event Action<LevelSystem, int> OnLevelChanged;

    private int level;
    private float exp;
    private float levelExp; // 레벨업에 필요한 경험치


    public int Level
    {
        get => level;
        private set // 레벨시스템 내에서만 수정 가능
        {
            exp -= levelExp;
            levelExp *= Settings.expPerLevel; // 레벨당 1.04씩 곱해짐 (4% 증가)

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
        Debug.Assert(owner != null, $"level::Setup - Owner는 Null이 될 수 없습니다.");

        Player = owner;
        level = 1; // 로드할 데이터가 있으면 무조건 로드를 하므로 1레벨로 셋업
        exp = 0;
        levelExp = Settings.startExp;
    }

    // 한번에 대량의 경험치를 획득했을때 호출 (미접속 보상)
    public void GetExpReward(int exp)
    {
        int levelUpCount = 0;
        float finalExp = this.exp + exp; // 기존 보유경험치 + 얻은 경험치

        // 현재 가진 경험치가 레벨업에 필요한 경험치보다 작을때까지
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

        // 기본레벨 : 1 , 로드한 레벨 : 40
        // levelExp는 총 39레벨만큼 증가해야함 (= expPerLevel의 39제곱)
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