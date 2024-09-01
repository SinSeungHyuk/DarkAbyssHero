using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DealDamageAction : EffectAction
{
    // 데미지를 결정할 스탯 종류
    [SerializeField] private Stat stat;
    // 기본 데미지 (1.0 = 100%) 
    [SerializeField] private float defaultDamage = 1.0f;
    // 레벨당 상승할 데미지 계수 (0.1 = 레벨당 10% 상승)
    [SerializeField] private float bonusDamagePerLevel;
 

    // Example
    // stat의 Value = 100 (공격력)
    // defaultDamage = 1.0 (데미지 계수 공격력의 100%)
    // effect.DataBonusLevel = 4 (사용 데이터 1레벨, 실제 스킬레벨 5레벨)
    // bonusDamagePerLevel = 0.1 (레벨당 상승 데미지)
    // 실제 데미지 => 100 * ((4*0.1) + 1.0) = 140
    public override bool Apply(Effect effect, Player player, Monster target, int level)
    {
        float totalDamage;
        bool isCritic = false;

        totalDamage = player.Stats.GetValue(stat) * ((effect.DataBonusLevel * bonusDamagePerLevel) + defaultDamage);

        // 치명타 발생했을 경우 치명타 데미지만큼 추가로 곱해줌
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
        // 계수가 1.4라면 140으로 리턴
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
