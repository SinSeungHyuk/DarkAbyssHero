using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Stats : MonoBehaviour
{
    [SerializeField] private Stat hpStat; // �������� HP����
    [SerializeField] private List<Stat> defaultStats = new List<Stat>(6); // �ִ� 6���� ����

    private List<Stat> stats = new List<Stat>(6);

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

        HPStat = hpStat ? GetStat(hpStat) : null;
    }

    private void OnDestroy()
    {

    }

    public Stat GetStat(Stat stat)
    {
        // �Ű������� ���� stat ID�� �̿�
        // stats ����Ʈ�� ���� ID �������� ��ȯ
        Debug.Assert(stat != null, "Stat is null!!!!");
        return stats.FirstOrDefault(x => x.ID == stat.ID);
    }
    public Stat GetStat(int id)
    {
        return stats.FirstOrDefault(x => x.ID == id);
    }

    public float GetValue(Stat stat) 
        => GetStat(stat).Value;


    public void SetDefaultValue(Stat stat, float value)
        => GetStat(stat).DefaultValue = value;
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
    public void SetDefaultValueByPercent(Stat stat,object key, float value)
        => GetStat(stat).SetValueByPercent(key, value);   
}
