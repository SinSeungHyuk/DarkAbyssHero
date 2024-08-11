using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnSkillObjectAction : SkillAction
{
    // ������ ��ų ������Ʈ
    [SerializeField] private GameObject skillObjectPrefab;

    // ��ų ������Ʈ�� ������
    [SerializeField] private float duration;
    [SerializeField] private float applyCount;
    [SerializeField] private Vector3 objectScale = Vector3.one;


    public override void Apply(Skill skill)
    {
        SkillObject skillObject = GameObject.Instantiate(skillObjectPrefab).GetComponent<SkillObject>();
        
        // ���� �Ĺ����� �ʵ��� ��¦ y������ �������
        skillObject.transform.position = skill.TargetPosition + (Vector3.up * 0.01f);
        skillObject.SetUp(skill, duration, applyCount, objectScale);
    }

    public override object Clone()
    {
        return new SpawnSkillObjectAction()
        {
            applyCount = applyCount,
            duration = duration,
            objectScale = objectScale,
            skillObjectPrefab = skillObjectPrefab,
        };
    }
}
