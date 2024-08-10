using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillAction : ICloneable
{
    public virtual void Start(Skill skill) { }
    public abstract void Apply(Skill skill);
    public virtual void Release(Skill skill) { }


    //public virtual void ApplyEffects(Skill skill, Entity target) 
    //{ 
    //    foreach (Effect effect in skill.Effects)
    //    {
    //        effect.Start();

    //        if (effect.IsApplicable) 
    //            effect.Apply();
    //    }
    //}

    
    public abstract object Clone();
}
