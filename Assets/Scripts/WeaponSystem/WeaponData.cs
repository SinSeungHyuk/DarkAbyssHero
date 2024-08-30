using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponData
{
    public int level; // �� ���� �������� ����

    [SerializeField] private Stat stat; // ���Ⱑ ��½����� ����
    [SerializeField] private float defaultValue; // ���� �⺻����
    [SerializeField] private float bonusStatPerLevel; // ������ ����


    public Stat Stat => stat; // ���Ⱑ ��½����� ����
    // ���������� �÷��� ������ ���ʽ� ����
    public float BonusStatValue =>
        defaultValue + (bonusStatPerLevel * (level-1));
}
