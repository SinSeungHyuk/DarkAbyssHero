using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// StateMachineBehaviour : �ִϸ����͸� ��Ʈ�����ִ� Ŭ����
public class EntityBaseLayerBehaviour : StateMachineBehaviour
{
    private Entity entity;
    private NavMeshAgent agent;


    // �ִϸ����Ϳ��� �� ���¿� ó�� ���Խ� ȣ��
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        // animator : �� �ִϸ����Ͱ� ����� ������Ʈ�� �����ؼ� �ٸ� ������Ʈ ������ �� ����
        entity = animator.GetComponent<Entity>();
        agent = animator.GetComponent<NavMeshAgent>();
    }

    // �ִϸ����Ϳ��� ���°� ������Ʈ�ɶ����� ȣ��
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // speed �Ķ���ʹ� 0~1 ������ ���� ���� (1�� �������� �ٴ� ���)
        // �ٴ� ��Ǹ� ���͵� ������� ������ agent.speed�� �״�� ����
        // ���� agent.speed�� 1���� Ŀ�� �ִ� 1�� �����Ǿ �Ķ���Ϳ� ��
        if (agent)
            animator.SetFloat(Settings.speed, agent.speed);

        //animator.SetBool(Settings.isDead, entity.i)
    }      
           


    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{      
    //}      
          
}
