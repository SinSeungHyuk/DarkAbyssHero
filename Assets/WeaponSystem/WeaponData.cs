using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponData
{
    [SerializeField] private Stat stat; // 무기가 상승시켜줄 스탯
    [SerializeField] private float defaultValue; // 무기 기본스탯
    [SerializeField] private float bonusStatPerLevel; // 레벨당 스탯
}
