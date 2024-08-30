using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapon : IdentifiedObject, ISaveData<WeaponSaveData>
{
    // <무기, 현재레벨, 이전레벨>
    public event Action<Weapon, int, int> OnLevelChanged;

    // 무기의 등급
    [SerializeField] private GradeType gradeType;
    [SerializeField] private WeaponData[] weaponDatas;

    private WeaponData currentData;

    private int defaultLevel = 1; // 기본레벨
    private int level; // 현재 레벨

    public Player Player { get; private set; }
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

            // 새로운 Level과 가장 가까운 Level Data를 찾아옴
            //var newData = skillDatas.Last(x => x.level <= level);
            ////if (newData.level != currentData.level)
            //ChangeData(newData);

            //OnLevelChanged?.Invoke(this, level, prevLevel);
        }
    }
    //public int DataBonusLevel => Mathf.Max(level - currentData.level, 0);
    public bool IsMaxLevel => level == MaxLevel;


    public void SetUp(Player owner, int level)
    {
        Debug.Assert(owner != null, $"Weapon::Setup - Owner는 Null이 될 수 없습니다.");
        Debug.Assert(Player == null, $"Weapon::Setup - 이미 Setup하였습니다.");

        Player = owner;
        Level = level;

        WeaponGrade = new Grade(gradeType); // gradeType으로 무기의 등급 생성
    }

    // 무기의 데이터를 레벨에 맞는 데이터로 교체
    //private void ChangeData(WeaponData newData)
    //{
    //    // 기존의 Effect들 파괴
    //    foreach (var effect in Effects)
    //        Destroy(effect);

    //    currentData = newData;

    //    Effects = currentData.effectSelectors.Select(x => x.CreateEffect(this)).ToArray();
    //    // Skill의 현재 Level이 data의 Level보다 크면, 둘의 Level 차를 Effect의 Bonus Level 줌.
    //    // 만약 Skill이 2 Level이고, data가 1 level이라면, effect들은 2-1해서 1의 Bonus Level을 받게 됨.
    //    if (level > currentData.level)
    //        UpdateCurrentEffectLevels();

    //    UpdateCustomActions();
    //}
    //private void UpdateCurrentEffectLevels()
    //{
    //    int bonusLevel = DataBonusLevel;
    //    foreach (var effect in Effects)
    //        effect.Level = Mathf.Min(effect.Level + bonusLevel, effect.MaxLevel);
    //}



    public void FromSaveData(WeaponSaveData saveData)
    {
        throw new System.NotImplementedException();
    }
    public WeaponSaveData ToSaveData()
    {
        throw new System.NotImplementedException();
    }
}

[Serializable]
public struct WeaponSaveData
{

}