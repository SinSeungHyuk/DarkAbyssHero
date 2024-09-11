using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;



public class Player : Entity, IDamageable, ISaveData<PlayerSaveData>
{
    // 스킬의 발사 위치 등을 찾기위한 딕셔너리
    private Dictionary<string, Transform> socketsByName = new();
    // HP 리젠을 위한 1초 간격의 WaitForSeconds 캐싱
    private readonly WaitForSeconds _wait = new WaitForSeconds(1f);
    private DamageEvent damageEvent;

    public DamageEvent DamageEvent => damageEvent;
    public Animator Animator { get; private set; }
    public Stats Stats { get; private set; }
    public bool IsDead => Stats.HPStat.DefaultValue <= 0f;
    public PlayerStateMachine StateMachine { get; private set; }
    public SkillSystem SkillSystem { get; private set; }
    public WeaponSystem WeaponSystem { get; private set; }
    public EntityMovement Movement { get; private set; }
    public CurrencySystem CurrencySystem { get; private set; }
    public LevelSystem LevelSystem { get; private set; }

    // 플레이어가 공격할 몬스터 대상
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

        WeaponSystem = GetComponent<WeaponSystem>();
        WeaponSystem?.SetUp(this);
    }

    private void Start()
    {
        // 게임이 시작되면 첫번째 스테이지부터 우선 생성 
        StageManager.Instance.CreateStage(0);
        StageManager.Instance.OnStageChanged += OnStageChanged;

        SaveManager.Instance.LoadGame(); // 데이터 로드
        StartCoroutine(HPRegenRoutine());
    }

    private void OnStageChanged(Stage stage, int level)
    {
        // 스테이지 변경시 항상 (0,0,0)으로 이동
        this.transform.position = Vector3.zero;
    }

    private IEnumerator HPRegenRoutine()
    {
        // 1초마다 체력재생 무한반복
        while (true)
        {
            Stats.HPStat.DefaultValue += Stats.GetStat(StatType.HPRegen).Value;

            yield return _wait;
        }
    }

    public void SetTarget(Monster target)
    {
        // 몬스터 타겟을 찾았을때
        Target = target;
        transform.LookAt(Target.transform);
        Movement.TraceTarget = target.transform;
    }

    private void OnDead() // 플레이어의 사망 애니메이션 이벤트
    {
        // UI 띄우기
        GameManager.Instance.PlayerDead();
    }

    public void OnRevive() // 게임매니저에서 호출할 부활함수
    {
        // 체력 풀피로 부활
        Stats.HPStat.DefaultValue = Stats.HPStat.MaxValue;

        // 체력재생 다시시작 (죽을때 꺼놨기 때문)
        StartCoroutine(HPRegenRoutine());
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
    // 저장되어있는 Socket을 가져오거나 순회를 통해 찾아옴
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
    public void TakeDamage(float damage, bool isCritic = false)
    {
        if (IsDead) return;

        Stats.HPStat.DefaultValue -= damage;
        damageEvent.CallTakeDamageEvent(damage);

        if (Stats.HPStat.DefaultValue <= 0f)
            StopCoroutine(HPRegenRoutine()); // 체력재생 스탑
    }

    public PlayerSaveData ToSaveData()
    {
        var saveData = new PlayerSaveData();

        saveData.LevelData = LevelSystem.ToSaveData();
        saveData.StatDatas = Stats.ToSaveData();
        saveData.CurrencyData = CurrencySystem.ToSaveData();
        saveData.SkillDatas = SkillSystem.ToSaveData();
        saveData.WeaponDatas = WeaponSystem.ToSaveData();
        saveData.StageData = StageManager.Instance.CurrentStage.ID; // 스테이지 ID 저장

        return saveData;
    }

    public void FromSaveData(PlayerSaveData saveData)
    {
        LevelSystem.FromSaveData(saveData.LevelData);
        Stats.FromSaveData(saveData.StatDatas);
        CurrencySystem.FromSaveData(saveData.CurrencyData);
        SkillSystem.FromSaveData(saveData.SkillDatas);
        WeaponSystem.FromSaveData(saveData.WeaponDatas);
        StageManager.Instance.CreateStage(saveData.StageData); // 저장된 스테이지ID 생성
    }
    #endregion
}

[Serializable]
public struct PlayerSaveData
{
    public LevelSaveData LevelData;
    public List<StatSaveData> StatDatas;
    public CurrencySaveData CurrencyData;
    public SkillSaveDatas SkillDatas;
    public WeaponSaveDatas WeaponDatas;
    public int StageData;
}
