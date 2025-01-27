
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

        // 랜덤으로 스폰간격 설정
        float spawnTimer = Random.Range(Settings.spawnTimerMin, Settings.spawnTimerMax);
        _wait = new WaitForSeconds(spawnTimer);

        // 미리 스폰할 몬스터의 종류를 정해놓고 코루틴 시작
        SetRandomSpawnMonster();
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        int i = 0;
        while (true)
        {
            // 미리 만든 randomEnemy 리스트에서 스폰할 몬스터 선택
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
        // 오브젝트 풀에 반환하기 위해 구독한 이벤트 해지
        damageEvent.OnDead -= DamageEvent_OnDead;

        // 이 스테이지에서 몇마리를 처치했는지 카운팅
        currentStage.RewardForMonsterKills();
    }

    private void SetRandomSpawnMonster()
    {
        // totalRatio : 몬스터의 스폰확률 전부 더한 값
        int totalRatio = monsterParameters.Sum(x => x.Ratio);

        // 총 100개의 몬스터 리스트 미리 생성
        for (int i = 0; i < 100; i++)
        {
            // 난수, 현재 몬스터의 스폰확률 누적값
            int randomNumber = Random.Range(0, totalRatio);
            int ratioSum = 0;

            foreach (var monster in monsterParameters)
            {
                // 현재 순회중인 몬스터가 난수에 포함되면 스폰당첨
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
