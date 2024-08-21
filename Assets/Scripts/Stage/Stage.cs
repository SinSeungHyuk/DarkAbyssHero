using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : IdentifiedObject
{
    // 생성할 Stage 프리팹
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private int stageLevel;
    [SerializeField] private List<MonsterSpawnParameter> monsterParameters = new();

    public GameObject StagePrefab => stagePrefab;
    public int StageRequiredLevel => stageLevel;
    public IReadOnlyList<MonsterSpawnParameter> MonsterParameters => monsterParameters;
    public int StageLevel => ID + 1;
}

[Serializable] // 각 스테이지마다 몬스터의 정보 직접입력
public struct MonsterSpawnParameter
{
    public string Name; // 풀에서 꺼낼 몬스터 이름
    public float Hp;
    public float Attack;
    public int Gold; // 몬스터가 드랍할 골드
    public int Exp; // 몬스터가 드랍할 경험치
    public int Ratio; // 몬스터가 스폰될 확률
}