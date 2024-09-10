using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstantApplyAction : SkillAction
{
    private SoundEffectSO soundEffect;


    public override void Start(Skill skill)
    {
        soundEffect = skill.SkillSound;
    }

    public override void Apply(Skill skill)
    {
        // Instant 스킬은 타겟이 플레이어의 타겟과 동일
        skill.Target = skill.Player.Target;
        skill.Target.EffectSystem.Apply(skill);
        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
    }

    public override object Clone() => new InstantApplyAction();
}
