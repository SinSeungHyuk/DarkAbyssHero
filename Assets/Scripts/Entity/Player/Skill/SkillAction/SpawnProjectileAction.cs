using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnProjectileAction : SkillAction
{
    // 투사체 프리팹
    [SerializeField] private GameObject projectilePrefab;
    // 투사체를 발사할 소켓의 이름 (직접입력)
    [SerializeField] private string spawnPointSocketName;
    // 투사체 속도
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
