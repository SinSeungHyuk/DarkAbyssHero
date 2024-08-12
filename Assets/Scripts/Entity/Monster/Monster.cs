using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity, IDamageable
{
    [SerializeField] private Transform player;

    private EntityMovement movement;
    private EffectSystem effectSystem;

    public EffectSystem EffectSystem => effectSystem;


    public bool IsDead => Stats.HPStat.DefaultValue <= 0f;
    public Stats Stats { get; private set; }

    float timer = 0.0f;


    public void Init()
    {
        movement = GetComponent<EntityMovement>();
        effectSystem = GetComponent<EffectSystem>();

        movement.SetUp(this);

        Stats = GetComponent<Stats>();
        Stats.SetUp(this);

    }

    void Start()
    {
        //Init();
        Debug.Log($"{IsDead} , {Stats.HPStat.DefaultValue}");

    }

    void Update()
    {
        movement.TraceTarget = player;

        //timer += Time.deltaTime;

        //if (timer > 5.0f) ObjectPoolManager.Instance.ReturnGameObject(gameObject,"Monster");
    }

    private void OnDestroy()
    {

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
