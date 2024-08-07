using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillData
{
    // ��ų�� ����
    public int level;


    // ��ų�׼� : ����ü, ������Ʈ ��ȯ, Ÿ�ٿ��� ��� ���� 
    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    public SkillAction action;

    [UnderlineTitle("Setting")]
    public SkillRunningFinishOption runningFinishOption;
    // runningFinishOption�� FinishWhenDurationEnded�̰� duration�� 0�̸� ���� ����
    [Min(0)]
    public float duration;
    // applyCount�� 0�̸� ���� ����
    [Min(0)]
    public int applyCount;
    // ù �ѹ��� ȿ���� �ٷ� ����� ���̱� ������, �ѹ� ����� �ĺ��� ApplyCycle�� ���� �����
    // ���� ��, ApplyCycle�� 1�ʶ��, �ٷ� �ѹ� ����� �� 1�ʸ��� ����ǰ� ��. 
    [Min(0f)]
    public float applyCycle;

    public float cooldown;

    public float distance; // ��ų�� ��Ÿ�


    // ĳ���� ��ų�� ����
    [UnderlineTitle("Cast")]
    public bool isUseCast;
    public float castTime;


    // �� ��ų�� ȿ���� ���� ����Ʈ��
    // EffectSelector���� Clone���� �����ؼ� ȿ�� �Ѱ���
    [UnderlineTitle("Effect")]
    public EffectSelector[] effectSelectors;

    // ��ų �ִϸ����� �Ķ����
    [UnderlineTitle("Animation")]
    public AnimatorParameter castAnimatorParameter;
    public AnimatorParameter actionAnimatorParameter;

    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCast;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnAction;
}
