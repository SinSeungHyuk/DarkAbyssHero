using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    // ù��° Apply�� �ٷ� �����Ű�� �ʰ� �������ϱ�
    [SerializeField] private bool isDelayFirstApplyByCycle;
    // ���ӽð��� ������ ���� ����Ŭ�� �ı��ϱ� (������ ����Ŭ ��ƼŬ �����ϱ�)
    [SerializeField] private bool isDelayDestroyByCycle;

    // �� ������Ʈ�� �ε��� ������Ʈ�� ���� (����Ŭ���� �˻�)
    private HashSet<Monster> collidingObjects = new HashSet<Monster>();
    // �ı��� ���͸� ��Ƽ� �ѹ��� ó���ϴ� ť
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
        // ������Ʈ �ı��� �����̽�Ű�� �ɼ��̶�� ����Ŭ��ŭ �ڷ� �̷�
        this.duration = duration + (isDelayDestroyByCycle ? applyCycle : 0f);
        var localScale = this.transform.localScale;
        this.transform.localScale = Vector3.Scale(localScale, objectScale);
    }

    private float CalcApplyCycle(float duration, float applyCount)
    {
        // 1�� �����̶�� ����Ŭ�� �ʿ����
        // ������ 0���� �ϸ� OnTriggerEnter���� ���� ȣ��� �� �����Ƿ� 0.1��
        if (applyCount == 1) return 0.01f;
        // ù ���ö��̸� �ǳʶٴ��� �ƴ����� ���� ����Ŭ ����
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
            Debug.Log(monster.name);

            // �̹� ���Ͱ� ���� ���¶�� Apply ��� ť�� �ֱ�
            if (monster.IsDead) deadMonster.Enqueue(monster);
            else
            {
                skill.Target = monster;
                monster.EffectSystem.Apply(skill);
            }
        }

        // ��ȸ�� ���� �� ť�� �ִ� ���͵� ó��
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
        Debug.Log("Trigger!!!");
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
