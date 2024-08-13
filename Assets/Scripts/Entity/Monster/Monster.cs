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
public class Monster : Entity, IDamageable
{
    //[SerializeField] 
    private Transform player;

    private EntityMovement movement;
    private Animator animator;
    private MonsterStateMachine stateMachine;
    private EffectSystem effectSystem;

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
        stateMachine = GetComponent<MonsterStateMachine>();
    }

    public void Init()
    {
        player = GameManager.Instance.GetPlayer().transform;

        movement.SetUp(this);
        Stats.SetUp(this);

        IsAttacking = false;
        timer = 0f;
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        movement.TraceTarget = player;

        if (timer < 1f)
        {
            timer += Time.fixedDeltaTime;
            return;
        }

        if (!IsAttacking)
        {
            animator.SetTrigger(Settings.isAttack);
        }
    }

    private void OnDestroy()
    {

    }

    private void ApplyMonsterAttack()
    {
        Debug.Log(Stats.GetStat(StatType.Attack).Value);
    } 


    #region Interface
    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        Stats.HPStat.DefaultValue -= damage;
        Debug.Log($"{gameObject.name} + TakeDamage : {Stats.HPStat.DefaultValue} , {damage}");

        if (Stats.HPStat.DefaultValue <= 0f)
            OnDead();
    }
    public void OnDead()
    {
        ObjectPoolManager.Instance.ReturnGameObject(gameObject, "Monster");
    }
    #endregion
}
