using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    [SerializeField] private Image imgBar;
    private Monster monster;

    private void Awake()
    {
        monster = GetComponentInParent<Monster>();
    }

    private void OnEnable()
    {
        monster.DamageEvent.OnTakeDamage += DamageEvent_OnTakeDamage;
    }
    private void OnDisable()
    {
        monster.DamageEvent.OnTakeDamage -= DamageEvent_OnTakeDamage;
    }

    private void DamageEvent_OnTakeDamage(DamageEvent @event, TakeDamageEventArgs args)
    {
        // ü�¹��� fillAmount�� ü�� ������ŭ ����
        imgBar.fillAmount = monster.Stats.GetHPStatRatio();
    }
}
