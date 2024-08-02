using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class Player : Entity, IDamageable, IEntity
{
    // ��ų�� �߻� ��ġ ���� ã������ ��ųʸ�
    private Dictionary<string, Transform> socketsByName = new();

    public Animator Animator { get; private set; }
    public DamageEvent DamageEvent { get; private set; }
    public Stats Stats { get; private set; }
    //public MonoStateMachine<Entity> StateMachine { get; private set; }
    //public SkillSystem SkillSystem { get; private set; }
    public EntityMovement Movement { get; private set; }
    public Monster Target { get; set; } // �÷��̾ ������ ���� ���



    private void Awake()
    {
        Animator = GetComponent<Animator>();

        DamageEvent = GetComponent<DamageEvent>();

        Movement = GetComponent<EntityMovement>();
        Movement.SetUp(this);

        Stats = GetComponent<Stats>();
        Stats.SetUp(this);

        //StateMachine = GetComponent<MonoStateMachine<Entity>>();
        //StateMachine?.SetUp(this);

        //SkillSystem = GetComponent<SkillSystem>();
        //SkillSystem?.SetUp(this);
    }

    private void Start()
    {
        Debug.Log($"{Stats.GetStat(0).Value} , {Stats.GetStat(1).Value}");
    }

    void Update()
    {
        
    }


    public void TakeDamage(float damage)
    {
        // Stats.HPStat.Value -= damage;

        DamageEvent.CallTakeDamageEvent(damage);
    }

    public void OnDead()
    {
        
    }
}
