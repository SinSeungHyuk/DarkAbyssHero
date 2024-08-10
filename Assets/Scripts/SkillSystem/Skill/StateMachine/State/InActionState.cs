using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InActionState : SkillState
{
    private bool isInstantApplyType;

    protected override void Awake()
    {
        isInstantApplyType = TOwner.ApplyType == SkillApplyType.Instant;
    }

    public override void Enter()
    {
        if (!TOwner.IsActivated)
            TOwner.Activate();

        // ó�� ��ų�� �׼ǻ��¿� �����ϸ鼭 SkillAction�� Start ����
        // ����ü �߻� ��ų�׼� ���� Start ������ �ȵǾ�����
        TOwner.StartSkillAction();

        // ó�� ��ų �׼ǻ��¿� �����ϸ鼭 Apply���� �� �� ���ֱ�
        Apply();
    }

    public override void Update()
    {
        // ���� ����� ��ų�� Duration, ApplyCycle �����ֱ�
        TOwner.CurrentDuration += Time.deltaTime;
        TOwner.CurrentApplyCycle += Time.deltaTime;

        // ��ų�� ���� �����ϴٸ� Apply�� ���� ���� (IsApplicable = ����Ƚ��,���� ��)
        if (TOwner.IsApplicable)
            Apply();
    }

    public override void Exit()
    {
        TOwner.ReleaseSkillAction();

        // �÷��̿��� ��ų�� �����ٴ� �޼��� ������ 
        // TrySendCommandToPlayer �� �ƴ϶� SendMessage �̹Ƿ� ���� �������� ������Ʈ���� ���޵�
        TOwner.Player.StateMachine.SendMessage(EntityStateMessage.FinishSkill);
    }

    private void Apply()
    {
        // �÷��̾�� �� ��ų�� ��������� ToInSkillActionState�� �����϶�� ���� ������
        TrySendCommandToPlayer(TOwner, PlayerStateCommand.ToInSkillActionState, TOwner.ActionAnimationParameter);

        // Skill�� Apply �Լ��� �� ��ų�� ������Ʈ���� ȣ���ϰų� Ȥ��
        // �ִϸ��̼��� ����Ǹ鼭 �ڵ����� �ִϸ��̼��� Ư�� Ÿ�ֿ̹��� ȣ���ϰų�
        if (isInstantApplyType)
            TOwner.Apply(); // Skill�� Apply �Լ� ȣ��
    }
}