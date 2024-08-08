using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTargetState : State<Player>
{
    protected override void Awake()
    {
    }

    public override void Enter()
    {
        Debug.Log("MoveToTargetState enterrrrrrrrrr");
    }

    public override void Exit()
    {
        //skill.Use();
        Debug.Log("MoveToTargetState exit");
    }
}