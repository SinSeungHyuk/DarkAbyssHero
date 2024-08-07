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
        Debug.Log("InActionState Enter!!");

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
        TOwner.CurrentDuration += Time.deltaTime;
        TOwner.CurrentApplyCycle += Time.deltaTime;

        // ��ų�� ���� �����ϴٸ� Apply�� ���� ����
        if (TOwner.IsApplicable)
            Apply();
    }

    public override void Exit()
    {
        Debug.Log("InActionState Exit!!");
        TOwner.ReleaseSkillAction();
    }

    private void Apply()
    {
        // �÷��̾�� �� ��ų�� ��������� ToInSkillActionState�� ��ȯ�϶�� ��� ������
        TrySendCommandToPlayer(TOwner, PlayerStateCommand.ToInSkillActionState, TOwner.ActionAnimationParameter);

        // Skill�� Apply �Լ��� �� ��ų�� ������Ʈ���� ȣ���ϰų� Ȥ��
        // �ִϸ��̼��� ����Ǹ鼭 �ڵ����� �ִϸ��̼��� Ư�� Ÿ�ֿ̹��� ȣ���ϰų�
        if (isInstantApplyType)
            TOwner.Apply();
        else 
            TOwner.CurrentApplyCount++;
    }
}