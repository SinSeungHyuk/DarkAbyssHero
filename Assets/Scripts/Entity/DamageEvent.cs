using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageEvent : MonoBehaviour
{
    // TakeDamage �̺�Ʈ�� OnDead �̺�Ʈ�� ���� Ŭ����
    // �������� ������ �״� �͵� ���� ������� ������ ���� �ϳ��� ����

    public event Action<DamageEvent, TakeDamageEventArgs> OnTakeDamage;
    public event Action<DamageEvent> OnDead;

    public void CallTakeDamageEvent(float damage, bool isCritic = false)
    {
        OnTakeDamage?.Invoke(this, new TakeDamageEventArgs()  { Damage = damage , isCritic = isCritic });
    }

    public void CallDeadEvent()
    {
        OnDead?.Invoke(this);
    }
}

public class TakeDamageEventArgs : EventArgs
{
    public float Damage;
    public bool isCritic;
}