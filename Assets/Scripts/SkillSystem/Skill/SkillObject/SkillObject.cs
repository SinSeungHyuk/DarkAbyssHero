using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    // 첫번째 Apply를 바로 적용시키지 않고 딜레이하기
    [SerializeField] private bool isDelayFirstApplyByCycle;
    // 지속시간이 끝나도 다음 사이클때 파괴하기 (마지막 사이클 파티클 무시하기)
    [SerializeField] private bool isDelayDestroyByCycle;

    // 이 오브젝트와 부딪힌 오브젝트들 저장 (사이클마다 검사)
    private HashSet<Monster> collidingObjects = new HashSet<Monster>();
    // 파괴된 몬스터를 담아서 한번에 처리하는 큐
    private Queue<Monster> deadMonster = new Queue<Monster>();

    private float duration;
    private float applyCount;
    private float applyCycle;

    private float currentDuration;
    private float currentApplyCycle;
    private int currentApplyCount;

    private Skill skill; 

    private bool isApplicable => (currentApplyCount < applyCount)
        && (currentApplyCycle >= applyCycle);


    public void SetUp(Skill skill, float duration, float applyCount, Vector3 objectScale)
    {
        this.skill = skill;
        this.applyCount = applyCount;
        this.applyCycle = CalcApplyCycle(duration, applyCount);
        // 오브젝트 파괴를 딜레이시키는 옵션이라면 사이클만큼 뒤로 미룸
        this.duration = duration + (isDelayDestroyByCycle ? applyCycle : 0f);
        var localScale = this.transform.localScale;
        this.transform.localScale = Vector3.Scale(localScale, objectScale);
    }

    private float CalcApplyCycle(float duration, float applyCount)
    {
        // 1번 적용이라면 사이클이 필요없음
        // 하지만 0으로 하면 OnTriggerEnter보다 빨리 호출될 수 있으므로 0.1초
        if (applyCount == 1) return 0.1f;
        // 첫 어플라이를 건너뛰는지 아닌지에 따라 사이클 조정
        else
            return isDelayFirstApplyByCycle ? (duration / applyCount) 
                : (duration / (applyCount - 1));
    }

    private void Update()
    {
        currentApplyCycle += Time.deltaTime;
        currentDuration += Time.deltaTime;

        if (isApplicable)
            Apply();

        if (currentDuration >= duration)
            Destroy(gameObject);    
    }

    private void Apply()
    {
        foreach (var monster in collidingObjects)
        {
            if (monster.IsDead) deadMonster.Enqueue(monster);
            else
            {
                skill.Target = monster;
                monster.EffectSystem.Apply(skill);
            }
        }

        DestroyDeadMonsters();

        currentApplyCount++;
        currentApplyCycle %= applyCycle;
    }

    private void DestroyDeadMonsters()
    {
        while (deadMonster.Count > 0)
        {
            Monster monster = deadMonster.Dequeue();
            collidingObjects.Remove(monster);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Monster monster = other.GetComponent<Monster>();
        if (monster != null)
            collidingObjects.Add(monster);
    }
    private void OnTriggerExit(Collider other)
    {
        Monster monster = other.GetComponent<Monster>();
        if (monster != null)
            collidingObjects.Remove(monster);
    }
}
