using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.UI.GridLayoutGroup;


public class Player : Entity, IDamageable, ISaveData<PlayerSaveData>
{
    // ��ų�� �߻� ��ġ ���� ã������ ��ųʸ�
    private Dictionary<string, Transform> socketsByName = new();

    public Animator Animator { get; private set; }
    public DamageEvent DamageEvent { get; private set; }
    public bool IsDead => Stats.HPStat.DefaultValue <= 0f;
    public Stats Stats { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public SkillSystem SkillSystem { get; private set; }
    public EntityMovement Movement { get; private set; }

    // �÷��̾ ������ ���� ���
    public Monster Target { get; private set; }



    private void Awake()
    {
        Animator = GetComponent<Animator>();

        DamageEvent = GetComponent<DamageEvent>();

        Movement = GetComponent<EntityMovement>();
        Movement.SetUp(this);

        Stats = GetComponent<Stats>();
        Stats.SetUp(this);

        StateMachine = GetComponent<PlayerStateMachine>();
        StateMachine?.SetUp(this);

        SkillSystem = GetComponent<SkillSystem>();
        SkillSystem?.SetUp(this);
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SkillSystem.EquipSkills[0].Level++;
            Debug.Log(SkillSystem.EquipSkills[0].Level);

            Monster monster = ObjectPoolManager.Instance.GetGameObject("Monster_Titan", Vector3.zero, Quaternion.identity).GetComponent<Monster>();
            monster.Init();

            Monster monster1 = ObjectPoolManager.Instance.GetGameObject("Monster_Creep", Vector3.zero, Quaternion.identity).GetComponent<Monster>();
            monster1.Init();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Stats.HPStat.DefaultValue += 10.0f;

            Debug.Log(Stats.HPStat.DefaultValue);
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            Stats.GetStat(StatType.Attack).SetValueByPercent("test",0.2f);

            Debug.Log(Stats.GetStat(StatType.Attack).Value);
        }
    }

    public void SetTarget(Monster target)
    {
        Target = target;
        transform.LookAt(Target.transform);
        Movement.TraceTarget = target.transform;
    }

    #region Find Transform Socket By SocketName
    // root transform�� �ڽ� transform���� ��ȸ�ϸ� �̸��� socketName�� GameObject�� Transform�� ã�ƿ� 
    private Transform GetTransformSocket(Transform root, string socketName)
    {
        if (root.name == socketName)
            return root;

        // root transform�� �ڽ� transform���� ��ȸ
        foreach (Transform child in root)
        {
            // ����Լ��� ���� �ڽĵ� �߿� socketName�� �ִ��� �˻���
            var socket = GetTransformSocket(child, socketName);
            if (socket)
                return socket;
        }

        return null;
    }
    // ������ִ� Socket�� �������ų� ��ȸ�� ���� ã�ƿ�
    public Transform GetTransformSocket(string socketName)
    {
        // dictionary���� socketName�� �˻��Ͽ� �ִٸ� return
        if (socketsByName.TryGetValue(socketName, out var socket))
            return socket;

        // dictionary�� �����Ƿ� ��ȸ �˻�
        socket = GetTransformSocket(transform, socketName);
        // socket�� ã���� dictionary�� �����Ͽ� ���Ŀ� �ٽ� �˻��� �ʿ䰡 ������ ��
        if (socket)
            socketsByName[socketName] = socket;

        return socket;
    }
    #endregion

    #region Interface
    public void TakeDamage(float damage)
    {
        // Stats.HPStat.Value -= damage;

        DamageEvent.CallTakeDamageEvent(damage);
    }

    public void OnDead()
    {
        
    }

    public PlayerSaveData ToSaveData()
    {
        throw new NotImplementedException();
    }

    public void FromSaveData(PlayerSaveData saveData)
    {
        throw new NotImplementedException();
    }
    #endregion
}

[Serializable]
public struct PlayerSaveData
{

}
