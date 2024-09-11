using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using DG.Tweening;
using UnityEngine.UIElements;

public class FloatingTextView : MonoBehaviour
{
    private TextMeshPro TxtDamage;
    private Transform txtTransform;

    public void InitializeDamageText(float damageAmount, bool isCritic, float yPos)
    {
        TxtDamage = GetComponent<TextMeshPro>();
        txtTransform = GetComponent<Transform>();
        txtTransform.rotation = Quaternion.Euler(23,0,0); // 카메라를 바라보는 방향의 각도로 조정

        // 일반공격 : 흰색글씨, 빨리 사라짐
        // 크리티컬 : 주황색글씨, 천천히 사라짐

        TxtDamage.text = damageAmount.ToString("0"); // 소수점 제거

        this.gameObject.SetActive(true);

        if (isCritic)
        {
            TxtDamage.color = Settings.critical;
            TxtDamage.fontSize = 3;

            // DOTween의 DOMoveY : Y축만 0.6만큼 1초에 걸쳐서 이동
            txtTransform.DOMoveY(yPos + 0.6f, 1f).SetEase(Ease.InOutQuad)
                .OnComplete(() => ObjectPoolManager.Instance.Release(gameObject, "FloatingText"));
        }
        else
        {
            TxtDamage.color = Color.white;
            TxtDamage.fontSize = 2;

            txtTransform.DOMoveY(yPos + 0.4f, 0.7f).SetEase(Ease.InOutQuad)
                .OnComplete(() => ObjectPoolManager.Instance.Release(gameObject, "FloatingText"));
        }
    }
}
