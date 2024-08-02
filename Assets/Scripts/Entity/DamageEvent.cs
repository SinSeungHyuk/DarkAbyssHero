using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageEvent : MonoBehaviour
{
    // TakeDamage 이벤트와 OnDead 이벤트를 가진 클래스
    // 데미지를 받으면 죽는 것도 같이 따라오기 때문에 둘을 하나로 통합

    public event Action<DamageEvent, TakeDamageEventArgs> OnTakeDamage;
    public event Action<DamageEvent> OnDead;

    public void CallTakeDamageEvent(float damage)
    {
        OnTakeDamage?.Invoke(this, new TakeDamageEventArgs()  { Damage = damage });
    }

    public void CallDeadEvent()
    {
        OnDead?.Invoke(this);
    }
}

public class TakeDamageEventArgs : EventArgs
{
    public float Damage;
}