using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private Rigidbody rigidBody;
    private float speed;
    private Skill skill;
    private SoundEffectSO soundEffect;
    private bool isPiercing;

    private float distance;
    private float currentDistance;
    private Vector3 distanceVector;


    public void SetUp(float speed, bool isPiercing, Vector3 forward, Skill skill)
    {
        soundEffect = skill.SkillSound;
        this.speed = speed;
        this.isPiercing = isPiercing;
        transform.forward = forward; // 투사체의 전방 설정
        this.skill = skill; // 투사체 날라가는 중 스킬레벨 상승하면 반영됨
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        distance = Settings.projectileDistance;
    }

    private void FixedUpdate()
    {
        // FixedUpdate에서 물리적 연산으로 전방을 향해 speed만큼 이동
        distanceVector = transform.forward * speed;
        currentDistance += distanceVector.magnitude; // 날아간 거리계산을 위한 magnitude
        rigidBody.velocity = distanceVector;

        if (currentDistance > distance) 
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var entity = other.GetComponent<Monster>();
        if (entity)
        {
            SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
            skill.Target = entity;
            entity.EffectSystem.Apply(skill);
        }

        if (!isPiercing) Destroy(gameObject);
    }
}
