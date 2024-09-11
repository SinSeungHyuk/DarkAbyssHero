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


    // ��ų�� ������ ���� ���ο� ����Ʈ�� �����ϴ� �Լ�
    public Effect CreateEffect(Skill owner)
    {
        Effect clone = effect.Clone() as Effect;
        clone.SetUp(owner, owner.Player, level);

        return clone;
    }
}
