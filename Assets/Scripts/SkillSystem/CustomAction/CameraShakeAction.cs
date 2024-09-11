using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraShakeAction : CustomAction
{
    // 미리 세팅해놓은 시네머신 카메라의 Shake 효과 발동하기
    public override void Run(object data)
        => Camera.main.GetComponent<Cinemachine.CinemachineImpulseSource>().GenerateImpulse();

    public override object Clone() => new CameraShakeAction();
}