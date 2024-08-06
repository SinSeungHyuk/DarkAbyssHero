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
