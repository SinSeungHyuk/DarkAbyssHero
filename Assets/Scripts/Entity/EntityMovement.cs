using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform traceTarget; // 추적 대상

    public Entity Owner { get; private set; }
    public Transform TraceTarget
    {
        get => traceTarget;
        set
        {
            agent.SetDestination(value.transform.position);

            traceTarget = value;
        }
    }
    public float StopDistance
    {
        get => agent.stoppingDistance;
        set => agent.stoppingDistance = value;
    }
    public bool IsStop => agent.remainingDistance <= agent.stoppingDistance;
    public float Speed => agent.speed;


    public void SetUp(Entity owner)
    {
        Owner = owner;

        agent = Owner.GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (traceTarget == null) return;  

        agent.SetDestination(traceTarget.transform.position);
    }

    // 이동 멈추고 추적 대상 비우기
    public void Stop()
    {
        traceTarget = null;

        if (agent.isOnNavMesh)
            agent.ResetPath();

        agent.velocity = Vector3.zero;
    }
}
