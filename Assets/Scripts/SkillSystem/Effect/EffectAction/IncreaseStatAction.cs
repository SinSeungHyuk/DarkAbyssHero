using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class IncreaseStatAction : EffectAction
{
    [SerializeField] private Stat stat; // ������ ������ ����
    [SerializeField] private float bonusValuePercent; // 1 = 100%
    [SerializeField] private float bonusValuePerLevel; // ��ų ������ ����� �߰�����
    [SerializeField] private bool isUndoOnRelease = true; // ��ų�� ������ ���� �ǵ�����


    public override bool Apply(Effect effect, Player user, Monster target, int level)
    {
        // EffectType�� Buff�� Debuff�Ŀ� ���� ����� �޶���

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

        // EffectType�� Buff�� Debuff�Ŀ� ���� ����� �޶���

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
