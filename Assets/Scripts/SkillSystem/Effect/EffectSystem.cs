using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EffectSystem : MonoBehaviour
{
    // ���� �������� Effect�� 
    private List<Effect> runningEffects = new List<Effect>();
    // ������ ��ġ�� �ı��� ����Ʈ ť
    private Queue<Effect> destroyEffectQueue = new Queue<Effect>();


    // ���� Ȥ�� �÷��̾�
    public Entity Owner { get; private set; }
    public IReadOnlyList<Effect> RunningEffects => runningEffects;


    private void OnDestroy()
    {
        // Effect ��ü�� �ı��ؼ� �޸� ���� ����
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

            effect.Update(); // Effect�� Update ���� ȣ��

            if (effect.IsFinished) 
                RemoveEffect(effect);
        }
    }

    private bool RemoveEffect(Effect effect)
    {
        if (effect == null || destroyEffectQueue.Contains(effect))
            return false;

        effect.Release();

        // ��ٷ� �ı��ϴ°� �ƴ϶� ť�� �ְ� �� ���� �Լ����� �ı�
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
    // SkillAction�� ����ü,������Ʈ ��� ȣ��� Apply �Լ� (�� ��ų�� ������ ���)
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
            // �̹� ��������Ʈ�� �ִٸ� ������ ����� ���� ����
            RemoveEffect(runningEffect);
            ApplyNewEffect(effect);
        }
    }

    private void ApplyNewEffect(Effect effect)
    {
        Effect newEffect = effect.Clone() as Effect;

        // ����Ʈ�� ������ ��ų�� Ÿ������ ����Ʈ Ÿ�� ����
        newEffect.SetTarget(effect.Skill.Target);

        newEffect.Start(); // ����Ʈ�׼�,Ŀ���Ҿ׼��� Start ȣ��

        if (newEffect.IsApplicable)
            newEffect.Apply();

        if (newEffect.IsFinished)
        {
            // ����Ʈ ����Ƚ�� 1ȸ�� ����ǰ� ��ٷ� Release �� �ı�
            newEffect.Release();
            Destroy(newEffect);
        }
        else runningEffects.Add(newEffect);
    }
    #endregion



    public Effect Find(Effect effect)
    => runningEffects.Find(x => x.ID == effect.ID);
}
