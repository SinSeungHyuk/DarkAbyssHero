using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Cinemachine.DocumentationSortingAttribute;

public class Weapon : IdentifiedObject, ISaveData<WeaponSaveData>
{
    // <무기, 현재레벨, 이전레벨>
    public event Action<Weapon, int, int> OnLevelChanged;

    // 무기의 등급
    [SerializeField] private GradeType gradeType;
    [SerializeField] private WeaponData[] weaponDatas;

    private int defaultLevel = 1; // 기본레벨
    private int level; // 현재 레벨

    public Player Player { get; private set; }
    public IReadOnlyList<WeaponData> CurrentDatas => weaponDatas;
    public GradeType GradeType => gradeType;
    public Grade WeaponGrade { get; private set; }

    public int MaxLevel => Settings.maxLevel;
    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value >= 1 && value <= MaxLevel,
                 $"Weapon.Rank = {value} - value는 1과 MaxLevel({MaxLevel}) 사이여야합니다.");

            if (level == value)
                return;

            int prevLevel = level;
            level = value;

            ChangeData();

            OnLevelChanged?.Invoke(this, level, prevLevel);
        }
    }
    public bool IsMaxLevel => level == MaxLevel;


    public void SetUp(Player owner, int level)
    {
        Debug.Assert(owner != null, $"Weapon::Setup - Owner는 Null이 될 수 없습니다.");
        Debug.Assert(Player == null, $"Weapon::Setup - 이미 Setup하였습니다.");

        Player = owner;
        Level = level;

        WeaponGrade = new Grade(gradeType); // gradeType으로 무기의 등급 생성
    }

    private void ChangeData()
    {
        // 현재 무기의 레벨에 맞추어 데이터 레벨 세팅
        for (int i=0;i< weaponDatas.Length; i++)
        {
            weaponDatas[i].level = Math.Min(level, MaxLevel);
        }
    }



    public void FromSaveData(WeaponSaveData saveData)
    {
        Level = saveData.level;
    }

    public WeaponSaveData ToSaveData()
            => new WeaponSaveData
            {
                id = ID,
                level = level
            };
}

[Serializable]
public struct WeaponSaveData
{
    public int id;
    public int level;
}
