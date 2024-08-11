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

    // �������̺� ��� �ڷ�ƾ�� ���� -> UniTask�� ��ü
    // ����� �½�ũ�� �ٽ� �޸� �����ϱ� ���� ��ū
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
        // ������ ��ū�� �����½�ũ�� ���� (Forget : ���޼��� ����)
        UniTask.Delay(TimeSpan.FromMilliseconds(1000), cancellationToken: cts.Token)
            .ContinueWith(() => DetectMonster(cts.Token))
            .Forget();
    }

    public override void Exit()
    {
        IsFindSkill = false;

        // �� ������Ʈ�� ����鼭 ��ū�� Cancel�ϰ� Dispose�ؼ� �޸� ����
        // ���۾��� ���ϸ� ��� �޸𸮿� ���Ƽ� �����߻�
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private async UniTaskVoid DetectMonster(CancellationToken cancellationToken)
    {
        // cts?.Cancel(); �۾� ��Ҹ� ��û�Ҷ����� �ݺ�
        while (!cancellationToken.IsCancellationRequested)
        {
            // OverlapSphere �Լ��� �ش� ���̾��� ������Ʈ�� ��������
            Collider[] hit = Physics.OverlapSphere(TOwner.transform.position, detectionRadius, monsterLayer);
            float closest = Mathf.Infinity;
            closestTarget = null;

            foreach (Collider monster in hit)
            {
                // ���� ��귮�� ���� sqrMagnitude �Լ� ��� : �Ÿ��񱳸� �ϸ� �ǹǷ�
                float distance = (monster.transform.position - TOwner.transform.position).sqrMagnitude;
                if (closest > distance)
                {
                    closest = distance;
                    closestTarget = monster;
                }
            }

            if (closestTarget != null)
            {
                // ���� ����� Ÿ���� ã�Ƽ� �÷��̾��� Ÿ������ �����ϱ�
                Monster monster = closestTarget.GetComponent<Monster>();
                TOwner.SetTarget(monster);

                IsFindSkill = TOwner.SkillSystem.FindUsableSkill();
            }

            try
            {
                // 0.5�� ������
                await UniTask.Delay(500, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
}

