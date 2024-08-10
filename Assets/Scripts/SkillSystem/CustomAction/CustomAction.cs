using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public abstract class CustomAction : ICloneable
{
    public virtual void Start(object data) { }
    public virtual void Run(object data) { }
    public virtual void Release(object data) { }

    public abstract object Clone();
}
