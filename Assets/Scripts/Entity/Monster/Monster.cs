using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Entity, IDamageable
{
    [SerializeField] private Transform player;

    private EntityMovement movement;
    private EffectSystem effectSystem;

    public EffectSystem EffectSystem => effectSystem;


    private float hp = 100.0f;

    float timer = 0.0f;


    public void Init()
    {
        timer = 0f;
    }

    void Start()
    {
        movement = GetComponent<EntityMovement>();
        effectSystem = GetComponent<EffectSystem>();

        movement.SetUp(this);
    }

    void Update()
    {
        //movement.TraceTarget = player;

        //timer += Time.deltaTime;

        //if (timer > 5.0f) ObjectPoolManager.Instance.ReturnGameObject(gameObject,"Monster");
    }

    private void OnDestroy()
    {

    }


    #region Interface
    public void TakeDamage(float damage)
    {
        Debug.Log($"{gameObject.name} + TakeDamage : {damage}");
        hp -= damage;
        if (hp <= 0 ) { OnDead(); }
    }
    public void OnDead()
    {
        //ObjectPoolManager.Instance.ReturnGameObject(gameObject, "Monster");
    }
    #endregion
}
