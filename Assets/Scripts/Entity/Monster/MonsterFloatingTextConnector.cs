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
        var floatingText = ObjectPoolManager.Instance.Get("FloatingText", new Vector3(txtSpawnPoint.position.x , yPos , txtSpawnPoint.position.z) , Quaternion.identity).GetComponent<FloatingTextView>();
        
        floatingText.InitializeDamageText(args.Damage, false, yPos);
    }
}
