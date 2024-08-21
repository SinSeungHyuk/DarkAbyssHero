using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : IdentifiedObject
{
    // ������ Stage ������
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private int stageLevel;
    [SerializeField] private List<MonsterSpawnParameter> monsterParameters = new();

    public GameObject StagePrefab => stagePrefab;
    public int StageRequiredLevel => stageLevel;
    public IReadOnlyList<MonsterSpawnParameter> MonsterParameters => monsterParameters;
    public int StageLevel => ID + 1;
}

[Serializable] // �� ������������ ������ ���� �����Է�
public struct MonsterSpawnParameter
{
    public string Name; // Ǯ���� ���� ���� �̸�
    public float Hp;
    public float Attack;
    public int Gold; // ���Ͱ� ����� ���
    public int Exp; // ���Ͱ� ����� ����ġ
    public int Ratio; // ���Ͱ� ������ Ȯ��
}