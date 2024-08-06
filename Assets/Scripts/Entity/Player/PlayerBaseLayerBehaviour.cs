using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// StateMachineBehaviour : 애니메이터를 컨트롤해주는 클래스
public class PlayerBaseLayerBehaviour : StateMachineBehaviour
{
    private Player entity;
    private NavMeshAgent agent;


    // 애니메이터에서 각 상태에 처음 진입시 호출
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        // animator : 이 애니메이터가 연결된 오브젝트에 접근해서 다른 컴포넌트 가져올 수 있음
        entity = animator.GetComponent<Player>();
        agent = animator.GetComponent<NavMeshAgent>();
    }

    // 애니메이터에서 상태가 업데이트될때마다 호출
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // agent.velocity.sqrMagnitude : 실제 agent의 현재속도가 들어감
        // 이 값이 1보다 커도 자동으로 speed 파라미터에는 1로 고정되어서 뛰는모션 재생
        if (agent)
            animator.SetFloat(Settings.speed, agent.velocity.sqrMagnitude);

        animator.SetBool(Settings.isDead, entity.IsDead);
    }      
           


    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{      
    //}      
          
}
