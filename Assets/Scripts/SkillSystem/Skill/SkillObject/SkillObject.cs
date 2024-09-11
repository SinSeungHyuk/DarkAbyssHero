using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using DG.Tweening;


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
    private SoundEffectSO soundEffect;

    private bool isApplicable => (currentApplyCount < applyCount)
        && (currentApplyCycle >= applyCycle);


    public void SetUp(Skill skill, float duration, float applyCount, Vector3 objectScale)
    {
        this.skill = skill;
        this.soundEffect = skill.SkillSound;
        this.applyCount = applyCount;
        this.applyCycle = CalcApplyCycle(duration, applyCount);
        // 오브젝트 파괴를 딜레이시키는 옵션이라면 사이클만큼 뒤로 미룸
        this.duration = duration + (isDelayDestroyByCycle ? applyCycle : 0f);
        var localScale = this.transform.localScale;
        this.transform.localScale = Vector3.Scale(localScale, objectScale);

        if (!isDelayFirstApplyByCycle)
        {
            // 스킬오브젝트가 처음 생성되자마자 스킬이 발동되어야함
            // 충돌체가 부딪혀서 해시셋에 들어갈 시간이 필요하므로 0.02초 딜레이
            DOVirtual.DelayedCall(0.02f, Apply);
        }
    }

    private float CalcApplyCycle(float duration, float applyCount)
    {
        // 1번 적용이라면 사이클이 필요없음
        // 하지만 0으로 하면 OnTriggerEnter보다 빨리 호출될 수 있으므로 0.1초
        if (applyCount == 1) return 0.01f;
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

        if (currentDuration > duration)
            Destroy(gameObject);    
    }

    public void Apply()
    {
        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);

        foreach (var monster in collidingObjects)
        {
            // 이미 몬스터가 죽은 상태라면 Apply 대신 큐에 넣기
            if (monster.IsDead) deadMonster.Enqueue(monster);
            else
            {
                skill.Target = monster;
                monster.EffectSystem.Apply(skill);
            }
        }

        // 순회가 끝난 후 큐에 있는 몬스터들 처리
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
        // GetComponent 보다 효율적으로 작동 (null을 반환하지 않으며 필요할때만 out 매개변수에 넣어줌)
        if (other.TryGetComponent(out Monster monster))
            collidingObjects.Add(monster);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Monster monster))
            collidingObjects.Remove(monster);
    }
}
