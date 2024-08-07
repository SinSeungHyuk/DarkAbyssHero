using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillData
{
    // 스킬의 레벨
    public int level;


    // 스킬액션 : 투사체, 오브젝트 소환, 타겟에게 즉시 적용 
    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    public SkillAction action;

    [UnderlineTitle("Setting")]
    public SkillRunningFinishOption runningFinishOption;
    // runningFinishOption이 FinishWhenDurationEnded이고 duration이 0이면 무한 지속
    [Min(0)]
    public float duration;
    // applyCount가 0이면 무한 적용
    [Min(0)]
    public int applyCount;
    // 첫 한번은 효과가 바로 적용될 것이기 때문에, 한번 적용된 후부터 ApplyCycle에 따라 적용됨
    // 예를 들어서, ApplyCycle이 1초라면, 바로 한번 적용된 후 1초마다 적용되게 됨. 
    [Min(0f)]
    public float applyCycle;

    public float cooldown;

    public float distance; // 스킬의 사거리


    // 캐스팅 스킬도 존재
    [UnderlineTitle("Cast")]
    public bool isUseCast;
    public float castTime;


    // 이 스킬의 효과를 담을 이펙트들
    // EffectSelector에서 Clone으로 복사해서 효과 넘겨줌
    [UnderlineTitle("Effect")]
    public EffectSelector[] effectSelectors;

    // 스킬 애니메이터 파라미터
    [UnderlineTitle("Animation")]
    public AnimatorParameter castAnimatorParameter;
    public AnimatorParameter actionAnimatorParameter;

    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCast;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnAction;
}
