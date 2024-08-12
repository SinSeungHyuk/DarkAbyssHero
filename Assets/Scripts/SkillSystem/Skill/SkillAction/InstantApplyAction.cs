using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstantApplyAction : SkillAction
{
    public override void Apply(Skill skill)
    {
        Debug.Log($"{skill.Target.name}");

        Debug.Assert(skill != null, "1 is null");
        Debug.Assert(skill.Target != null, "2 is null");
        Debug.Assert(skill.Target.EffectSystem != null, "3 is null");
        skill.Target.EffectSystem.Apply(skill);
    }

    public override object Clone() => new InstantApplyAction();
}
