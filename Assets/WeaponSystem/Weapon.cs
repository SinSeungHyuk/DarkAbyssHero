using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapon : IdentifiedObject, ISaveData<WeaponSaveData>
{
    // <����, ���緹��, ��������>
    public event Action<Weapon, int, int> OnLevelChanged;

    // ������ ���
    [SerializeField] private GradeType gradeType;
    [SerializeField] private WeaponData[] weaponDatas;

    private WeaponData currentData;

    private int defaultLevel = 1; // �⺻����
    private int level; // ���� ����

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
                 $"Weapon.Rank = {value} - value�� 1�� MaxLevel({MaxLevel}) ���̿����մϴ�.");

            if (level == value)
                return;

            int prevLevel = level;
            level = value;

            // ���ο� Level�� ���� ����� Level Data�� ã�ƿ�
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
        Debug.Assert(owner != null, $"Weapon::Setup - Owner�� Null�� �� �� �����ϴ�.");
        Debug.Assert(Player == null, $"Weapon::Setup - �̹� Setup�Ͽ����ϴ�.");

        Player = owner;
        Level = level;

        WeaponGrade = new Grade(gradeType); // gradeType���� ������ ��� ����
    }

    // ������ �����͸� ������ �´� �����ͷ� ��ü
    //private void ChangeData(WeaponData newData)
    //{
    //    // ������ Effect�� �ı�
    //    foreach (var effect in Effects)
    //        Destroy(effect);

    //    currentData = newData;

    //    Effects = currentData.effectSelectors.Select(x => x.CreateEffect(this)).ToArray();
    //    // Skill�� ���� Level�� data�� Level���� ũ��, ���� Level ���� Effect�� Bonus Level ��.
    //    // ���� Skill�� 2 Level�̰�, data�� 1 level�̶��, effect���� 2-1�ؼ� 1�� Bonus Level�� �ް� ��.
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