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
    private Transform rect;

    public void InitializeDamageText(float damageAmount, bool isCritic, float yPos)
    {
        TxtDamage = GetComponent<TextMeshPro>();
        rect = GetComponent<Transform>();
        rect.rotation = Quaternion.Euler(23,0,0);

        // ÀÏ¹Ý°ø°Ý : Èò»ö±Û¾¾, »¡¸® »ç¶óÁü
        // Å©¸®Æ¼ÄÃ : ÁÖÈ²»ö±Û¾¾, ÃµÃµÈ÷ »ç¶óÁü

        TxtDamage.text = damageAmount.ToString("0");

        this.gameObject.SetActive(true);

        if (isCritic)
        {
            TxtDamage.color = Settings.critical;
            TxtDamage.fontSize = 3;

            rect.DOMoveY(yPos + 0.6f, 1f).SetEase(Ease.InOutQuad)
                .OnComplete(() => ObjectPoolManager.Instance.Release(gameObject, "FloatingText"));
        }
        else
        {
            TxtDamage.color = Color.white;
            TxtDamage.fontSize = 2;

            rect.DOMoveY(yPos + 0.4f, 0.7f).SetEase(Ease.InOutQuad)
                .OnComplete(() => ObjectPoolManager.Instance.Release(gameObject, "FloatingText"));
        }
    }
}
