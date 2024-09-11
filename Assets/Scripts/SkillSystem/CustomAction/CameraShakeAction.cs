using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraShakeAction : CustomAction
{
    // �̸� �����س��� �ó׸ӽ� ī�޶��� Shake ȿ�� �ߵ��ϱ�
    public override void Run(object data)
        => Camera.main.GetComponent<Cinemachine.CinemachineImpulseSource>().GenerateImpulse();

    public override object Clone() => new CameraShakeAction();
}