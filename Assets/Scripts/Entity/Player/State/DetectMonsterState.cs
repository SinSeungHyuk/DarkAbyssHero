using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class DetectMonsterState : State<Player>
{
    public bool IsFindSkill { get; private set; }
    private float detectionRadius;
    private LayerMask monsterLayer;
    private Collider closestTarget;

    // 모노비헤이비어가 없어서 코루틴을 못씀 -> UniTask로 대체
    // 사용한 태스크를 다시 메모리 해제하기 위한 토큰
    private CancellationTokenSource cts;

    protected override void Awake()
    {
        detectionRadius = Settings.detectionRadius;
        monsterLayer = Settings.monsterLayer;
        IsFindSkill = false;
    }

    public override void Enter()
    {
        cts = new CancellationTokenSource();
        // 생성한 토큰의 유니태스크를 실행 (Forget : 경고메세지 무시)
        UniTask.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken: cts.Token)
            .ContinueWith(() => DetectMonster(cts.Token))
            .Forget();
    }

    public override void Exit()
    {
        IsFindSkill = false;

        // 이 스테이트를 벗어나면서 토큰을 Cancel하고 Dispose해서 메모리 비우기
        // 이작업을 안하면 계속 메모리에 남아서 누수발생
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private async UniTaskVoid DetectMonster(CancellationToken cancellationToken)
    {
        // cts?.Cancel(); 작업 취소를 요청할때까지 반복
        while (!cancellationToken.IsCancellationRequested)
        {
            // OverlapSphere 함수로 해당 레이어의 오브젝트들 가져오기
            Collider[] hit = Physics.OverlapSphere(TOwner.transform.position, detectionRadius, monsterLayer);
            float closest = Mathf.Infinity;
            closestTarget = null;

            foreach (Collider monster in hit)
            {
                // 가장 계산량이 적은 sqrMagnitude 함수 사용 : 거리비교만 하면 되므로
                float distance = (monster.transform.position - TOwner.transform.position).sqrMagnitude;
                if (closest > distance)
                {
                    closest = distance;
                    closestTarget = monster;
                }
            }

            if (closestTarget != null)
            {
                // 가장 가까운 타겟을 찾아서 플레이어의 타겟으로 설정하기
                Monster monster = closestTarget.GetComponent<Monster>();
                TOwner.SetTarget(monster);

                IsFindSkill = TOwner.SkillSystem.FindUsableSkill();
            }

            try
            {
                // 0.5초 딜레이
                await UniTask.Delay(500, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}

