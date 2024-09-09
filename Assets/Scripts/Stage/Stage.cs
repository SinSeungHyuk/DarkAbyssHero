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
    [SerializeField] private MusicTrackSO stageMusic;

    private int monsterKills = 0;

    public GameObject StagePrefab => stagePrefab;
    public MusicTrackSO StageMusic => stageMusic;
    public int StageRequiredLevel => stageLevel;
    public IReadOnlyList<MonsterSpawnParameter> MonsterParameters => monsterParameters;
    public int StageLevel => ID + 1;


    public (int,int) GetAvgRewards(int hours)
    {
        int avgGold = 0;
        int avgExp = 0;

        foreach (var monster in MonsterParameters)
        {
            avgGold += monster.Gold;
            avgExp += monster.Exp;
        }

        avgGold /= monsterParameters.Count;
        avgExp /= monsterParameters.Count;

        return (avgGold * 180 * hours, avgExp * 180 * hours);
    }

    public void RewardForMonsterKills()
    {
        // ���� ���ڰ� Ȱ��ȭ���̸� ����
        if (GameManager.Instance.IsChestActive) return;

        monsterKills++;

        if (monsterKills >= Settings.killsToReward)
        {
            monsterKills = 0;
            GameManager.Instance.SetRewardChest();
        }
    }
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