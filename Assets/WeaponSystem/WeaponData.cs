using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponData
{
    [SerializeField] private Stat stat; // ���Ⱑ ��½����� ����
    [SerializeField] private float defaultValue; // ���� �⺻����
    [SerializeField] private float bonusStatPerLevel; // ������ ����
}
