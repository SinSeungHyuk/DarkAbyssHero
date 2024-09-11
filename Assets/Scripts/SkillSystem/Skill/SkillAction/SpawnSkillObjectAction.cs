using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnSkillObjectAction : SkillAction
{
    // 생성할 스킬 오브젝트
    [SerializeField] private GameObject skillObjectPrefab;

    // 스킬 오브젝트의 데이터
    [SerializeField] private float duration;
    [SerializeField] private float applyCount;
    [SerializeField] private Vector3 objectScale = Vector3.one;


    public override void Apply(Skill skill)
    {
        SkillObject skillObject = GameObject.Instantiate(skillObjectPrefab).GetComponent<SkillObject>();
        
        // 땅에 파묻히지 않도록 살짝 y축으로 띄워놓기
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
