using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnProjectileAction : SkillAction
{
    // 투사체 프리팹
    [SerializeField] private GameObject projectilePrefab;
    // 투사체 속도
    [SerializeField] private float speed;
    // 투사체 통과 여부
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
