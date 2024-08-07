using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public class DetectMonsterState : State<Player>
{
    private float detectionRadius;
    private LayerMask monsterLayer;

    private EntityMovement movement;
    private Collider closestTarget;


    protected override void Awake()
    {
        detectionRadius = Settings.detectionRadius;
        monsterLayer = Settings.monsterLayer;

        movement = TOwner.Movement;
    }

    public override void Enter()
    {
        DetectMonster().Forget();
    }

    private async UniTaskVoid DetectMonster()
    {
        while (true) 
        {
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
                Debug.Log("hi");
                Monster monster = closestTarget.GetComponent<Monster>();
                TOwner.SetTarget(monster);
               
                Owner.ExecuteCommand(SkillExecuteCommand.Ready);
            }

            await UniTask.Delay(500);
        }
    } 
}
