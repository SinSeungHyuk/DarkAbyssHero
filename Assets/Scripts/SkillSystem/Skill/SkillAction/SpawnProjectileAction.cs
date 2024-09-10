using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnProjectileAction : SkillAction
{
    // ����ü ������
    [SerializeField] private GameObject projectilePrefab;
    // ����ü �ӵ�
    [SerializeField] private float speed;
    // ����ü ��� ����
    [SerializeField] private bool isPiercing;

    public override void Apply(Skill skill)
    {
        var socket = skill.Player.GetTransformSocket(Settings.shootPoint);
        var projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = socket.position;
        projectile.GetComponent<Projectile>().SetUp(speed,isPiercing, socket.forward, skill);
    }


    public override object Clone()
    {
        return new SpawnProjectileAction()
        {
            projectilePrefab = projectilePrefab,
            speed = speed,
            isPiercing = isPiercing
        };
    }
}
