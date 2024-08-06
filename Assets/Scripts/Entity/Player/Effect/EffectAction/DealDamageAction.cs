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
    private float GetTotalDamage(Effect effect, Player player)    
        => player.Stats.GetValue(stat) * ((effect.DataBonusLevel * bonusDamagePerLevel) + defaultDamage);

    public override bool Apply(Effect effect, Player player, Monster target, int level)
    {

        var totalDamage = GetTotalDamage(effect, player);
        target.TakeDamage(totalDamage);

        return true;
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
