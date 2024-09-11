using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectSelector 
{
    [SerializeField] private int level;
    [SerializeField] private Effect effect;

    public int Level => level;
    public Effect Effect => effect;


    // 스킬의 레벨에 따라 새로운 이펙트를 생성하는 함수
    public Effect CreateEffect(Skill owner)
    {
        Effect clone = effect.Clone() as Effect;
        clone.SetUp(owner, owner.Player, level);

        return clone;
    }
}
