
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private Stage currentStage;
    private IReadOnlyList<MonsterSpawnParameter> monsterParameters;
    private List<MonsterSpawnParameter> randomEnemy = new(100);
    private WaitForSeconds _wait;


    private void OnEnable()
    {
        StageManager.Instance.OnStageChanged += CurrentStage_OnStageChanged;
    }
    private void OnDisable()
    {
        StageManager.Instance.OnStageChanged -= CurrentStage_OnStageChanged;
        StopAllCoroutines();
    }

    private void CurrentStage_OnStageChanged(Stage stage, int level)
    {
        currentStage = stage;
        monsterParameters = stage.MonsterParameters;

        // �������� �������� ����
        float spawnTimer = Random.Range(Settings.spawnTimerMin, Settings.spawnTimerMax);
        _wait = new WaitForSeconds(spawnTimer);

        // �̸� ������ ������ ������ ���س��� �ڷ�ƾ ����
        SetRandomSpawnMonster();
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        int i = 0;
        while (true)
        {
            // �̸� ���� randomEnemy ����Ʈ���� ������ ���� ����
            MonsterSpawnParameter parameter = randomEnemy[i%100];

            Monster monster = ObjectPoolManager.Instance.Get(parameter.Name, transform).GetComponent<Monster>();
            monster.Init(parameter);
            monster.DamageEvent.OnDead += DamageEvent_OnDead;

            i++;
            yield return _wait;
        }
    }

    private void DamageEvent_OnDead(DamageEvent damageEvent)
    {
        // ������Ʈ Ǯ�� ��ȯ�ϱ� ���� ������ �̺�Ʈ ����
        damageEvent.OnDead -= DamageEvent_OnDead;

        // �� ������������ ����� óġ�ߴ��� ī����
        currentStage.RewardForMonsterKills();
    }

    private void SetRandomSpawnMonster()
    {
        // totalRatio : ������ ����Ȯ�� ���� ���� ��
        int totalRatio = monsterParameters.Sum(x => x.Ratio);

        // �� 100���� ���� ����Ʈ �̸� ����
        for (int i = 0; i < 100; i++)
        {
            // ����, ���� ������ ����Ȯ�� ������
            int randomNumber = UnityEngine.Random.Range(0, totalRatio);
            int ratioSum = 0;

            foreach (var monster in monsterParameters)
            {
                // ���� ��ȸ���� ���Ͱ� ������ ���ԵǸ� ������÷
                ratioSum += monster.Ratio;
                if (randomNumber < ratioSum)
                {
                    randomEnemy.Add(monster);
                    break;
                }
            }
        }
    }
}
