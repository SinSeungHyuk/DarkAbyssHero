using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnProjectileAction : SkillAction
{
    // ����ü ������
    [SerializeField] private GameObject projectilePrefab;
    // ����ü�� �߻��� ������ �̸� (�����Է�)
    [SerializeField] private string spawnPointSocketName;
    // ����ü �ӵ�
    [SerializeField] private float speed;

    public override void Apply(Skill skill)
    {
        var socket = skill.Player.GetTransformSocket(spawnPointSocketName);
        var projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = socket.position;
        //projectile.GetComponent<Projectile>().Setup(skill.Owner, speed, socket.forward, skill);
    }


    public override object Clone()
    {
        return new SpawnProjectileAction()
        {
            projectilePrefab = projectilePrefab,
            spawnPointSocketName = spawnPointSocketName,
            speed = speed
        };
    }
}
