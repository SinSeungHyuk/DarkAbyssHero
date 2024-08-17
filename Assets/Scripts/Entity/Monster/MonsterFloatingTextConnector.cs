using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MonsterFloatingTextConnector : MonoBehaviour
{
    private Transform txtSpawnPoint;
    private Monster monster;
    private float yPos;

    private void Start()
    {
        monster = GetComponentInParent<Monster>();
        txtSpawnPoint = GetComponent<Transform>();
        monster.DamageEvent.OnTakeDamage += OnTakeDamage;

        yPos = txtSpawnPoint.position.y;
    }

    private void OnTakeDamage(DamageEvent @event, TakeDamageEventArgs args)
    {
        // 데미지를 받을때 이전에 설정한 텍스트UI의 y위치를 넣어야 적절한 위치에 생성됨
        var floatingText = ObjectPoolManager.Instance.Get("FloatingText", new Vector3(txtSpawnPoint.position.x , yPos , txtSpawnPoint.position.z) , Quaternion.identity).GetComponent<FloatingTextView>();
        
        floatingText.InitializeDamageText(args.Damage, args.isCritic, yPos);
    }
}
