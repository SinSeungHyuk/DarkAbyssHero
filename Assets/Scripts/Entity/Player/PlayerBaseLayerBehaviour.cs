using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// StateMachineBehaviour : �ִϸ����͸� ��Ʈ�����ִ� Ŭ����
public class PlayerBaseLayerBehaviour : StateMachineBehaviour
{
    private Player entity;
    private NavMeshAgent agent;


    // �ִϸ����Ϳ��� �� ���¿� ó�� ���Խ� ȣ��
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        // animator : �� �ִϸ����Ͱ� ����� ������Ʈ�� �����ؼ� �ٸ� ������Ʈ ������ �� ����
        entity = animator.GetComponent<Player>();
        agent = animator.GetComponent<NavMeshAgent>();
    }

    // �ִϸ����Ϳ��� ���°� ������Ʈ�ɶ����� ȣ��
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // agent.velocity.sqrMagnitude : ���� agent�� ����ӵ��� ��
        // �� ���� 1���� Ŀ�� �ڵ����� speed �Ķ���Ϳ��� 1�� �����Ǿ �ٴ¸�� ���
        if (agent)
            animator.SetFloat(Settings.speed, agent.velocity.sqrMagnitude);

        animator.SetBool(Settings.isDead, entity.IsDead);
    }      
           


    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{      
    //}      
          
}
