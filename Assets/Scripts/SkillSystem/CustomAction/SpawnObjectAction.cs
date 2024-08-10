
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class SpawnObjectAction : CustomAction
{
    // 언제 Spawn 시킬지 (Run : Update에서 프레임마다 실행)
    private enum MethodType { Start, Run }
    // 누구를 기준으로 Object를 Spawn할지
    private enum TargetType { Player, Target }

    [SerializeField] private TargetType targetType;
    [SerializeField] private MethodType methodType;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 scaleFactor = Vector3.one;
    [SerializeField] private bool isAttachToTarget;
    [SerializeField] private bool isDestroyOnRelease;

    private GameObject spawnedObject;

    public override void Start(object data)
    {
        if (methodType == MethodType.Start)
            Spawn(data);
    }

    public override void Run(object data)
    {
        if (methodType == MethodType.Run)
            Spawn(data);
    }

    public override void Release(object data)
    {
        if (spawnedObject && isDestroyOnRelease)
            GameObject.Destroy(spawnedObject);
    }

    private GameObject Spawn(Vector3 position)
    {
        spawnedObject = GameObject.Instantiate(prefab);
        spawnedObject.transform.position = position + offset;
        var localScale = spawnedObject.transform.localScale;
        spawnedObject.transform.localScale = Vector3.Scale(localScale, scaleFactor);

        return spawnedObject;
    }

    private void Spawn(Transform transform)
    {
        var spawnedObject = Spawn(transform.position);
        if (isAttachToTarget)
            spawnedObject.transform.SetParent(transform, true);
    }

    private void Spawn(Skill data)
    {
        if (targetType == TargetType.Player)
            Spawn(data.Player.transform);
        else
            Spawn(data.TargetPosition);
    }

    private void Spawn(Effect data)
    {
        var targetTransform = targetType == TargetType.Player ? data.Player.transform : data.Target.transform;
        Spawn(targetTransform);
    }

    private void Spawn(object data)
    {
        if (data is Skill)
            Spawn(data as Skill);
        else
            Spawn(data as Effect);
    }

    public override object Clone()
    {
        return new SpawnObjectAction()
        {
            isAttachToTarget = isAttachToTarget,
            isDestroyOnRelease = isDestroyOnRelease,
            methodType = methodType,
            targetType = targetType,
            offset = offset,
            prefab = prefab,
            scaleFactor = scaleFactor,
        };
    }
}