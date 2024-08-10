using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// ����ȭ�� ���� �ڽ�Ŭ�������� �����Ϳ��� ������ �� �ְ�
[System.Serializable] 
public abstract class EffectAction : ICloneable
{
    // Effect�� ���۵� �� ȣ��Ǵ� ���� �Լ�
    public virtual void Start(Effect effect, Player user, Monster target, int level) { }
    // ���� Effect�� ȿ���� �����ϴ� �Լ�
    public abstract bool Apply(Effect effect, Player user, Monster target, int level);
    // Effect�� ����� �� ȣ��Ǵ� ���� �Լ�
    public virtual void Release(Effect effect, Player user, Monster target, int level) { }


    public abstract object Clone();
}
