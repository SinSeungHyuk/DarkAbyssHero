using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstantApplyAction : SkillAction
{
    public override void Apply(Skill skill)
    {
        // Instant 스킬은 타겟이 플레이어의 타겟과 동일
        skill.Target = skill.Player.Target;
        skill.Target.EffectSystem.Apply(skill);

        Debug.Assert(skill != null, "1 is null");
        Debug.Assert(skill.Target != null, "2 is null");
        Debug.Assert(skill.Target.EffectSystem != null, "3 is null");
    }

    public override object Clone() => new InstantApplyAction();
}
