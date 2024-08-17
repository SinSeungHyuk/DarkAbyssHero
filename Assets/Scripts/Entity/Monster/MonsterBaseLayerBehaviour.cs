using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBaseLayerBehaviour : StateMachineBehaviour
{
    private Monster entity;
    private NavMeshAgent agent;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        entity = animator.GetComponent<Monster>();
        agent = animator.GetComponent<NavMeshAgent>();

        // ������ �ִϸ��̼��� ���ݾִϸ��̼��̶��
        if (stateInfo.shortNameHash == Settings.AttackState)
            entity.IsAttacking = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent)
            animator.SetFloat(Settings.speed, agent.velocity.sqrMagnitude);

        animator.SetBool(Settings.isDead, entity.IsDead);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ���� �ִϸ��̼��� ���ݾִϸ��̼��̶��
        if (stateInfo.shortNameHash == Settings.AttackState)
            entity.IsAttacking = false;
    }
}