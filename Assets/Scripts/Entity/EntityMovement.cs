using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform traceTarget; // ���� ���

    public Entity Owner { get; private set; }
    public Transform TraceTarget
    {
        get => traceTarget;
        set
        {
            if (traceTarget == value) return;

            Stop();

            traceTarget = value;
            agent.SetDestination(traceTarget.position);
        }
    }
    public float StopDistance
    {
        get => agent.stoppingDistance;
        set => agent.stoppingDistance = value;
    }
    public bool IsStop => agent.stoppingDistance >= agent.remainingDistance;


    public void SetUp(Entity owner)
    {
        Owner = owner;

        agent = Owner.GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.destination = (traceTarget.transform.position);
    }
    private void Update()
    {

    }

    // �̵� ���߰� ���� ��� ����
    public void Stop()
    {
        traceTarget = null;

        if (agent.isOnNavMesh)
            agent.ResetPath();

        agent.velocity = Vector3.zero;
    }
}
