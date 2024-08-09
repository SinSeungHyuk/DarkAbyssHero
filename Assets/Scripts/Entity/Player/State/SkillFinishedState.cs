using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillFinishedState : State<Player>
{
    public bool IsStateEnded { get; private set; }
    private bool isFinishSkill;
    private AnimatorStateInfo lastStateInfo;
    private int LocomotionState;

    protected override void Awake() 
    {
        LocomotionState = Settings.LocomotionState;
        IsStateEnded = false;
        isFinishSkill = false;
    }

    public override void Enter()
    {

    }

    public override void Update()
    {
        if (!isFinishSkill) return;

        //Debug.Log(TOwner.Movement.Agent.desiredVelocity);
        lastStateInfo = TOwner.Animator.GetCurrentAnimatorStateInfo(0);

        if (lastStateInfo.shortNameHash == LocomotionState)
        {
            Debug.Log(lastStateInfo.shortNameHash == LocomotionState);
            IsStateEnded = true;
        }
    }

    public override void Exit()
    {
        IsStateEnded = false;
        isFinishSkill = false;
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // SkillState의 TrySendCommandToPlayer 함수를 통해 메세지 전달됨
        if ((EntityStateMessage)message != EntityStateMessage.FinishSkill)
            return false;


        Debug.Log("FinishSkill");

        isFinishSkill = true;

        return true;
    }
}
