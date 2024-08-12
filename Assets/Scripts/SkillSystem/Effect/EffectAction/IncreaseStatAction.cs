using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class IncreaseStatAction : EffectAction
{
    [SerializeField] private Stat stat; // 변경할 스탯의 종류
    [SerializeField] private float bonusValuePercent; // 1 = 100%
    [SerializeField] private float bonusValuePerLevel; // 스킬 레벨당 상승할 추가스탯
    [SerializeField] private bool isUndoOnRelease = true; // 스킬이 끝나면 스탯 되돌릴지


    public override bool Apply(Effect effect, Player user, Monster target, int level)
    {
        // EffectType이 Buff냐 Debuff냐에 따라 대상이 달라짐

        if (effect.EffectType == EffectType.Buff)
            user.Stats.GetStat(stat).SetValueByPercent(this, (effect.DataBonusLevel * bonusValuePerLevel) + bonusValuePercent);

        else if (effect.EffectType == EffectType.Debuff)
            target.Stats.GetStat(stat).SetValueByPercent(this, -((effect.DataBonusLevel * bonusValuePerLevel) + bonusValuePercent));
        

        return true;
    }

    public override void Release(Effect effect, Player user, Monster target, int level)
    {
        if (!isUndoOnRelease)
            return;

        // EffectType이 Buff냐 Debuff냐에 따라 대상이 달라짐

        if (effect.EffectType == EffectType.Buff)
            user.Stats.RemoveBonusValue(stat, this);
        else if (effect.EffectType == EffectType.Debuff)
            target.Stats.RemoveBonusValue(stat, this);
    }


    public override object Clone()
    {
        return new IncreaseStatAction()
        {
            stat = stat,
            bonusValuePercent = bonusValuePercent,
            bonusValuePerLevel = bonusValuePerLevel,
            isUndoOnRelease = isUndoOnRelease
        };
    }
}
