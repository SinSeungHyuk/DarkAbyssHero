using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 직렬화를 통해 자식클래스들을 에디터에서 선택할 수 있게
[System.Serializable] 
public abstract class EffectAction : ICloneable
{
    // Effect가 시작될 때 호출되는 시작 함수
    public virtual void Start(Effect effect, Player user, Monster target, int level) { }
    // 실제 Effect의 효과를 구현하는 함수
    public abstract bool Apply(Effect effect, Player user, Monster target, int level);
    // Effect가 종료될 때 호출되는 종료 함수
    public virtual void Release(Effect effect, Player user, Monster target, int level) { }


    public abstract object Clone();
}
