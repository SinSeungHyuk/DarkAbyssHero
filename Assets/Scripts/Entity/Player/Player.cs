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
    // HP ������ ���� 1�� ������ WaitForSeconds ĳ��
    private readonly WaitForSeconds _wait = new WaitForSeconds(1f);
    private DamageEvent damageEvent;

    public DamageEvent DamageEvent => damageEvent;
    public Animator Animator { get; private set; }
    public Stats Stats { get; private set; }
    public bool IsDead => Stats.HPStat.DefaultValue <= 0f;
    public PlayerStateMachine StateMachine { get; private set; }
    public SkillSystem SkillSystem { get; private set; }
    public EntityMovement Movement { get; private set; }
    public CurrencySystem CurrencySystem { get; private set; }
    public LevelSystem LevelSystem { get; private set; }

    // �÷��̾ ������ ���� ���
    public Monster Target { get; private set; }



    private void Awake()
    {
        Animator = GetComponent<Animator>();
        damageEvent = GetComponent<DamageEvent>();

        CurrencySystem = GetComponent<CurrencySystem>();
        CurrencySystem.SetUp(this);

        LevelSystem = GetComponent<LevelSystem>();
        LevelSystem.SetUp(this);

        Movement = GetComponent<EntityMovement>();
        Movement.SetUp(this);

        Stats = GetComponent<Stats>();
        Stats.SetUp(this);
        Stats.HPStat.DefaultValue = Stats.HPStat.MaxValue;

        StateMachine = GetComponent<PlayerStateMachine>();
        StateMachine?.SetUp(this);

        SkillSystem = GetComponent<SkillSystem>();
        SkillSystem?.SetUp(this);
    }

    private void Start()
    {
        //SaveManager.Instance.LoadGame();
        StageManager.Instance.OnStageChanged += OnStageChanged;
        StartCoroutine(HPRegenRoutine());

        damageEvent.CallTakeDamageEvent(0); // ü�� UI �ʱ�ȭ
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CurrencySystem.IncreaseCurrency(CurrencyType.SkillUp, 5000);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Stats.HPStat.DefaultValue += 10.0f;

            Debug.Log(Stats.HPStat.DefaultValue + " / " + Stats.HPStat.MaxValue);
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            Stats.HPStat.Level++;

            Debug.Log(Stats.HPStat.DefaultValue + " / " + Stats.HPStat.MaxValue);
        }
    }

    private void OnStageChanged(Stage stage, int level)
    {
        this.transform.position = Vector3.zero;
    }

    private IEnumerator HPRegenRoutine()
    {
        while (true)
        {
            yield return _wait;

            Stats.HPStat.DefaultValue += Stats.GetStat(StatType.HPRegen).Value;
        }
    }

    public void SetTarget(Monster target)
    {
        Target = target;
        transform.LookAt(Target.transform);
        Movement.TraceTarget = target.transform;
    }

    private void OnDead() // �÷��̾��� ��� �ִϸ��̼� �̺�Ʈ
    {

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
    public void TakeDamage(float damage, bool isCritic = false)
    {
        if (IsDead) return;

        Stats.HPStat.DefaultValue -= damage;
        damageEvent.CallTakeDamageEvent(damage);

        if (Stats.HPStat.DefaultValue <= 0f)
            StopCoroutine(HPRegenRoutine());
    }

    public PlayerSaveData ToSaveData()
    {
        var saveData = new PlayerSaveData();

        saveData.LevelData = LevelSystem.ToSaveData();
        saveData.StatDatas = Stats.ToSaveData();
        saveData.CurrencyData = CurrencySystem.ToSaveData();

        return saveData;
    }

    public void FromSaveData(PlayerSaveData saveData)
    {
        LevelSystem.FromSaveData(saveData.LevelData);
        Stats.FromSaveData(saveData.StatDatas);
        CurrencySystem.FromSaveData(saveData.CurrencyData);
    }
    #endregion
}

[Serializable]
public struct PlayerSaveData
{
    public LevelSaveData LevelData;
    public List<StatSaveData> StatDatas;
    public CurrencySaveData CurrencyData;
}

    //public SkillSaveData SkillData;