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
        // Instant ��ų�� Ÿ���� �÷��̾��� Ÿ�ٰ� ����
        skill.Target = skill.Player.Target;
        skill.Target.EffectSystem.Apply(skill);
        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
    }

    public override object Clone() => new InstantApplyAction();
}
