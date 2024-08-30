using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponData
{
    public int level; // 이 무기 데이터의 레벨

    [SerializeField] private Stat stat; // 무기가 상승시켜줄 스탯
    [SerializeField] private float defaultValue; // 무기 기본스탯
    [SerializeField] private float bonusStatPerLevel; // 레벨당 스탯


    public Stat Stat => stat; // 무기가 상승시켜줄 스탯
    // 최종적으로 올려줄 무기의 보너스 스탯
    public float BonusStatValue =>
        defaultValue + (bonusStatPerLevel * (level-1));
}
