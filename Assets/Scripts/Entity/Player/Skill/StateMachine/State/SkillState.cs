using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : State<Skill>
{
    // Skill을 소유한 Player의 StateMachine에게 상태 전환 Command와 SKill의 정보를 보내는 함수
    protected void TrySendCommandToOwner(Skill skill, PlayerStateCommand command, int animatorParameter)
    {
        var playerStateMachine = TOwner.Player.StateMachine;
        if (playerStateMachine != null)
        {
            // Transition이 Command를 받아들였으면, State로 UsingSKill Message와 Skill 정보를 보냄
            if (playerStateMachine.ExecuteCommand(command))
                playerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
        }
    }
}