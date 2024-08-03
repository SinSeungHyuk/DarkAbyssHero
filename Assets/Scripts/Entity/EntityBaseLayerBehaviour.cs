using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// StateMachineBehaviour : 애니메이터를 컨트롤해주는 클래스
public class EntityBaseLayerBehaviour : StateMachineBehaviour
{
    private Entity entity;
    private NavMeshAgent agent;


    // 애니메이터에서 각 상태에 처음 진입시 호출
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        // animator : 이 애니메이터가 연결된 오브젝트에 접근해서 다른 컴포넌트 가져올 수 있음
        entity = animator.GetComponent<Entity>();
        agent = animator.GetComponent<NavMeshAgent>();
    }

    // 애니메이터에서 상태가 업데이트될때마다 호출
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // speed 파라미터는 0~1 사이의 값을 가짐 (1에 가까울수록 뛰는 모션)
        // 뛰는 모션만 나와도 상관없기 때문에 agent.speed를 그대로 넣음
        // 만약 agent.speed가 1보다 커도 최대 1로 고정되어서 파라미터에 들어감
        if (agent)
            animator.SetFloat(Settings.speed, agent.speed);

        //animator.SetBool(Settings.isDead, entity.i)
    }      
           


    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{      
    //}      
          
}
