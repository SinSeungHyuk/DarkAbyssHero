using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private Stage currentStage;
    private IReadOnlyList<MonsterSpawnParameter> monsterParameters;
    private List<MonsterSpawnParameter> randomEnemy = new(100);


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
        monsterParameters = stage.MonsterParameters;

        SetRandomSpawnMonster();
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        int i = 0;
        while (true)
        {
            MonsterSpawnParameter parameter = randomEnemy[i%100];

            Monster monster = ObjectPoolManager.Instance.Get(parameter.Name, transform).GetComponent<Monster>();
            monster.Init(parameter);

            i++;

            yield return new WaitForSeconds(Settings.spawnTimer);
        }
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
