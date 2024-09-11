using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Stats : MonoBehaviour
{
    [SerializeField] private Stat hpStat; // 고정적인 HP스탯
    [SerializeField] private List<Stat> defaultStats = new List<Stat>(6); // 최대 6종류 스탯

    public List<Stat> stats = new List<Stat>(6);

    public Entity Owner { get; private set; }
    public Stat HPStat { get; private set; }
    


    public void SetUp(Entity entity)
    {
        Owner = entity;

        foreach (Stat stat in defaultStats)
        {
            Stat clone = stat.Clone() as Stat;
            stats.Add(clone);
        }

        HPStat = GetStat(hpStat);
    }

    private void OnDisable()
    {
        // 몬스터의 경우 Disable로 비활성화되기 때문에 스탯을 여기서 파괴시켜줌

        foreach (Stat stat in stats)
            Destroy(stat);
        stats.Clear();
    }

    public float GetHPStatRatio()
    {
        // 0~1 사이의 현재 체력비율 반환
        return HPStat.DefaultValue / HPStat.MaxValue;
    }

    public Stat GetStat(Stat stat)
    {
        // 매개변수로 받은 stat ID를 이용
        // stats 리스트의 같은 ID 스탯으로 반환
        Debug.Assert(stat != null, "Stat is null!!!!");
        return stats.FirstOrDefault(x => x.ID == stat.ID);
    }
    public Stat GetStat(int id)    
        => stats.FirstOrDefault(x => x.ID == id);
    public Stat GetStat(StatType statType)
    => GetStat(Convert.ToInt32(statType));


    public float GetValue(Stat stat) 
        => GetStat(stat).Value;
    public float GetValue(StatType statType)
    => GetStat(statType).Value;


    public void SetDefaultValue(Stat stat, float value)
        => GetStat(stat).DefaultValue = value;
    public void SetDefaultValue(StatType statType, float value)
    => GetStat(statType).DefaultValue = value;
    public float GetDefaultValue(Stat stat)
        => GetStat(stat).DefaultValue;

    public void IncreaseDefaultValue(Stat stat, float value)
        => GetStat(stat).DefaultValue += value;
    public void IncreaseBonusValue(Stat stat, object key, float value)
        => GetStat(stat).IncreaseBonusValue(key, value);

    public void RemoveBonusValue(Stat stat, object key)
        => GetStat(stat).RemoveBonusValue(key);

    public float GetBonusValue(Stat stat)
        => GetStat(stat).BonusValue;
    public void SetValueByPercent(Stat stat,object key, float value)
        => GetStat(stat).SetValueByPercent(key, value);



    // 각 Stat 종류마다 레벨,수치가 저장되기 때문에 이를 하나의 리스트로 묶어야함
    // 각 Stat의 정보가 저장된 리스트를 순회하며 id에 맞추어 데이터 넣어주기

    public List<StatSaveData> ToSaveData()
        => stats.Select(x => x.ToSaveData()).ToList();

    public void FromSaveData(List<StatSaveData> statDatas)
        => statDatas.ForEach(data => 
            stats.FirstOrDefault(x => x.ID == data.id).FromSaveData(data));
}
