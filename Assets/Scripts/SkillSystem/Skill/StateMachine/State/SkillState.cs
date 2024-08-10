using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : State<Skill>
{
    // Skill�� ������ Player�� StateMachine���� ���� ��ȯ Command�� SKill�� ������ ������ �Լ�
    protected void TrySendCommandToPlayer(Skill skill, PlayerStateCommand command, int animatorParameter)
    {
        var playerStateMachine = TOwner.Player.StateMachine;
        if (playerStateMachine != null)
        {
            // Transition�� Command�� �޾Ƶ鿴����, State�� UsingSkill Message�� Skill ������ ����
            if (playerStateMachine.ExecuteCommand(command))
            {
                playerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
            }
        }
    }
}