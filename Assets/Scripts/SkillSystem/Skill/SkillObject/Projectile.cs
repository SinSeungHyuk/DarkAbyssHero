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
        transform.forward = forward; // ����ü�� ���� ����
        this.skill = skill; // ����ü ���󰡴� �� ��ų���� ����ϸ� �ݿ���
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        distance = Settings.projectileDistance;
    }

    private void FixedUpdate()
    {
        // FixedUpdate���� ������ �������� ������ ���� speed��ŭ �̵�
        distanceVector = transform.forward * speed;
        currentDistance += distanceVector.magnitude; // ���ư� �Ÿ������ ���� magnitude
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
