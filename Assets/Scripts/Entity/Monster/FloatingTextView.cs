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
        txtTransform.rotation = Quaternion.Euler(23,0,0); // ī�޶� �ٶ󺸴� ������ ������ ����

        // �Ϲݰ��� : ����۾�, ���� �����
        // ũ��Ƽ�� : ��Ȳ���۾�, õõ�� �����

        TxtDamage.text = damageAmount.ToString("0"); // �Ҽ��� ����

        this.gameObject.SetActive(true);

        if (isCritic)
        {
            TxtDamage.color = Settings.critical;
            TxtDamage.fontSize = 3;

            // DOTween�� DOMoveY : Y�ุ 0.6��ŭ 1�ʿ� ���ļ� �̵�
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
