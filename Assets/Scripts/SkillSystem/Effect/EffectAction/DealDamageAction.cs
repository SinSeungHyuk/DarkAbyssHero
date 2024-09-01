using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DealDamageAction : EffectAction
{
    // �������� ������ ���� ����
    [SerializeField] private Stat stat;
    // �⺻ ������ (1.0 = 100%) 
    [SerializeField] private float defaultDamage = 1.0f;
    // ������ ����� ������ ��� (0.1 = ������ 10% ���)
    [SerializeField] private float bonusDamagePerLevel;
 

    // Example
    // stat�� Value = 100 (���ݷ�)
    // defaultDamage = 1.0 (������ ��� ���ݷ��� 100%)
    // effect.DataBonusLevel = 4 (��� ������ 1����, ���� ��ų���� 5����)
    // bonusDamagePerLevel = 0.1 (������ ��� ������)
    // ���� ������ => 100 * ((4*0.1) + 1.0) = 140
    public override bool Apply(Effect effect, Player player, Monster target, int level)
    {
        float totalDamage;
        bool isCritic = false;

        totalDamage = player.Stats.GetValue(stat) * ((effect.DataBonusLevel * bonusDamagePerLevel) + defaultDamage);

        // ġ��Ÿ �߻����� ��� ġ��Ÿ ��������ŭ �߰��� ������
        if (UtilitieHelper.isSuccess(player.Stats.GetStat(StatType.CriticChance).Value))
        {
            isCritic = true;
            totalDamage *= player.Stats.GetStat(StatType.CriticDamage).Value;
        }

        target.TakeDamage(totalDamage, isCritic);

        return true;
    }

    public override float GetEffectCoefficient(int level)
    {
        // ����� 1.4��� 140���� ����
        return ((level * bonusDamagePerLevel) + defaultDamage) * 100;
    }


    public override object Clone()
    {
        return new DealDamageAction()
        {
            stat = stat,
            defaultDamage = defaultDamage,
            bonusDamagePerLevel = bonusDamagePerLevel,
        };
    }
}
