using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.UI.GridLayoutGroup;


public class Player : Entity, IDamageable, ISaveData<PlayerSaveData>
{
    // 스킬의 발사 위치 등을 찾기위한 딕셔너리
    private Dictionary<string, Transform> socketsByName = new();

    public Animator Animator { get; private set; }
    public DamageEvent DamageEvent { get; private set; }
    public bool IsDead => Stats.HPStat.DefaultValue <= 0f;
    public Stats Stats { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public SkillSystem SkillSystem { get; private set; }
    public EntityMovement Movement { get; private set; }

    // 플레이어가 공격할 몬스터 대상
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
    // root transform의 자식 transform들을 순회하며 이름이 socketName인 GameObject의 Transform을 찾아옴 
    private Transform GetTransformSocket(Transform root, string socketName)
    {
        if (root.name == socketName)
            return root;

        // root transform의 자식 transform들을 순회
        foreach (Transform child in root)
        {
            // 재귀함수를 통해 자식들 중에 socketName이 있는지 검색함
            var socket = GetTransformSocket(child, socketName);
            if (socket)
                return socket;
        }

        return null;
    }
    // 저장되있는 Socket을 가져오거나 순회를 통해 찾아옴
    public Transform GetTransformSocket(string socketName)
    {
        // dictionary에서 socketName을 검색하여 있다면 return
        if (socketsByName.TryGetValue(socketName, out var socket))
            return socket;

        // dictionary에 없으므로 순회 검색
        socket = GetTransformSocket(transform, socketName);
        // socket을 찾으면 dictionary에 저장하여 이후에 다시 검색할 필요가 없도록 함
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
