using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Stats : MonoBehaviour
{
    [SerializeField] private Stat hpStat; // �������� HP����
    [SerializeField] private List<Stat> stats = new List<Stat>(6); // �ִ� 6���� ����

    public Entity Owner { get; private set; }
    public Stat HPStat { get; private set; }


    public void SetUp(Entity entity)
    {
        Owner = entity;

        HPStat = hpStat ? GetStat(hpStat) : null;
    }

    private void OnDestroy()
    {
        foreach (var stat in stats)
            Destroy(stat);
        stats = null;
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
    public void IncreaseBonusValue(Stat stat, float value)
        => GetStat(stat).IncreaseBonusValue(value);

    public float GetBonusValue(Stat stat)
        => GetStat(stat).BonusValue;
    public void SetDefaultValueByPercent(Stat stat, float value)
        => GetStat(stat).SetDefaultValueByPercent(value);   
}
