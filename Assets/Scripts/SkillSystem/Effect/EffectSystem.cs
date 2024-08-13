using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EffectSystem : MonoBehaviour
{
    // 현재 적용중인 Effect들 
    private List<Effect> runningEffects = new List<Effect>();
    // 적용을 마치고 파괴될 이펙트 큐
    private Queue<Effect> destroyEffectQueue = new Queue<Effect>();


    // 몬스터 혹은 플레이어
    public Entity Owner { get; private set; }
    public IReadOnlyList<Effect> RunningEffects => runningEffects;


    private void OnDestroy()
    {
        // Effect 객체들 파괴해서 메모리 누수 방지
        foreach (var effect in runningEffects)
            Destroy(effect);
    }

    private void OnDisable()
    {
        foreach (var effect in runningEffects)
            Destroy(effect);
    }

    private void Update()
    {
        UpdateRunningEffects();
        DestroyReleasedEffects();
    }

    #region Effect Update
    private void UpdateRunningEffects()
    {
        for (int i = 0; i < runningEffects.Count; i++)
        {
            Effect effect = runningEffects[i];

            if (effect.IsReleased) continue;

            effect.Update(); // Effect의 Update 수동 호출

            if (effect.IsFinished) 
                RemoveEffect(effect);
        }
    }

    private bool RemoveEffect(Effect effect)
    {
        if (effect == null || destroyEffectQueue.Contains(effect))
            return false;

        effect.Release();

        // 곧바로 파괴하는게 아니라 큐에 넣고 그 다음 함수에서 파괴
        destroyEffectQueue.Enqueue(effect);

        return true;
    }

    private void DestroyReleasedEffects()
    {
        while (destroyEffectQueue.Count > 0)
        {
            Effect effect = destroyEffectQueue.Dequeue();
            runningEffects.Remove(effect);
            Destroy(effect);
        }
    }
    #endregion


    #region Effect Apply
    // SkillAction의 투사체,오브젝트 등에서 호출될 Apply 함수 (그 스킬의 정보가 담김)
    public void Apply(Skill skill)
    {
        foreach (Effect effect in skill.Effects)
            Apply(effect);
    }

    public void Apply(Effect effect)
    {
        Effect runningEffect = Find(effect);

        if (runningEffect == null)
            ApplyNewEffect(effect);
        else
        {
            // 이미 러닝이펙트에 있다면 기존걸 지우고 새로 적용
            RemoveEffect(runningEffect);
            ApplyNewEffect(effect);
        }
    }

    private void ApplyNewEffect(Effect effect)
    {
        Effect newEffect = effect.Clone() as Effect;

        // 이펙트를 소유한 스킬의 타겟으로 이펙트 타겟 설정
        newEffect.SetTarget(effect.Skill.Target);

        newEffect.Start(); // 이펙트액션,커스텀액션의 Start 호출

        if (newEffect.IsApplicable)
            newEffect.Apply();

        if (newEffect.IsFinished)
        {
            // 이펙트 적용횟수 1회면 적용되고 곧바로 Release 후 파괴
            newEffect.Release();
            Destroy(newEffect);
        }
        else runningEffects.Add(newEffect);
    }
    #endregion



    public Effect Find(Effect effect)
    => runningEffects.Find(x => x.ID == effect.ID);
}
