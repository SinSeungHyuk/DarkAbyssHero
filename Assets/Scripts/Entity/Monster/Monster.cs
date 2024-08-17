using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(EntityMovement))]
[RequireComponent(typeof(EffectSystem))]
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DamageEvent))]
public class Monster : Entity, IDamageable
{
    //[SerializeField] 
    private Player player;

    private EntityMovement movement;
    private Animator animator;
    private EffectSystem effectSystem;
    private MonsterSpawnParameter monsterInfo;
    private DamageEvent damageEvent;

    public DamageEvent DamageEvent => damageEvent;
    public EffectSystem EffectSystem => effectSystem;
    public Stats Stats { get; private set; }
    public bool IsDead => Stats.HPStat.DefaultValue <= 0f;
    public bool IsAttacking { get; set; }

    private float timer;


    private void Awake()
    {
        movement = GetComponent<EntityMovement>();
        effectSystem = GetComponent<EffectSystem>();
        Stats = GetComponent<Stats>();
        animator = GetComponent<Animator>();
        damageEvent = GetComponent<DamageEvent>();
    }

    private void OnEnable()
    {
        damageEvent.OnDead += DamageEvent_OnDead;
    }
    private void OnDisable()
    {
        damageEvent.OnDead -= DamageEvent_OnDead;
    }

    public void Init(MonsterSpawnParameter parameter)
    {
        player = GameManager.Instance.GetPlayer();

        movement.SetUp(this);
        Stats.SetUp(this);

        monsterInfo = parameter;
        Stats.HPStat.MaxValue = parameter.Hp;
        Stats.SetDefaultValue(StatType.Attack, parameter.Attack);

        IsAttacking = false;
        timer = 0f;
        damageEvent.CallTakeDamageEvent(0); // ü�¹� UI �ʱ�ȭ
    }

    void FixedUpdate()
    {  
        // �÷��̾��� ��ġ�� ��� ã���ֱ�
        if (!IsDead)
            movement.TraceTarget = player.transform;

        if (timer < 1f) // ó�� �����ǰ� 1�ʵ��� ����x
        {
            timer += Time.fixedDeltaTime;
            return;
        }

        if (!IsAttacking)
            animator.SetTrigger(Settings.isAttack);
    }

    private void ApplyMonsterAttack() // ������ ���� �ִϸ��̼� �̺�Ʈ
    {
        player.TakeDamage(Stats.GetStat(StatType.Attack).Value);
    } 

    private void OnDead() // ������ ��� �ִϸ��̼� �̺�Ʈ
    {
        ObjectPoolManager.Instance.Release(gameObject, monsterInfo.Name);
    }

    private void DamageEvent_OnDead(DamageEvent @event)
    {
        player.Stats.HPStat.DefaultValue += player.Stats.GetStat(StatType.HPOnKill).Value;
        player.LevelSystem.Exp = monsterInfo.Exp;
        player.CurrencySystem.IncreaseCurrency(CurrencyType.Gold,monsterInfo.Gold);
    }


    #region Interface
    public void TakeDamage(float damage, bool isCritic = false)
    {
        if (IsDead) return;

        Stats.HPStat.DefaultValue -= damage;
        damageEvent.CallTakeDamageEvent(damage, isCritic);

        if (Stats.HPStat.DefaultValue <= 0f)
        {
            movement.Stop();
            damageEvent.CallDeadEvent();
        }
    }
    #endregion
}
