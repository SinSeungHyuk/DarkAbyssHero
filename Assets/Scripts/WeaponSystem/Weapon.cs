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

    private int defaultLevel = 1; // �⺻����
    private int level; // ���� ����

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
                 $"Weapon.Rank = {value} - value�� 1�� MaxLevel({MaxLevel}) ���̿����մϴ�.");

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
        Debug.Assert(owner != null, $"Weapon::Setup - Owner�� Null�� �� �� �����ϴ�.");
        Debug.Assert(Player == null, $"Weapon::Setup - �̹� Setup�Ͽ����ϴ�.");

        Player = owner;
        Level = level;

        WeaponGrade = new Grade(gradeType); // gradeType���� ������ ��� ����
    }

    private void ChangeData()
    {
        for (int i=0;i< weaponDatas.Length; i++)
        {
            weaponDatas[i].level = Math.Min(level, MaxLevel);
        }
    }



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