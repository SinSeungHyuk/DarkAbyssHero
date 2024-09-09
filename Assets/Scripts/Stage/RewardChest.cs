using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardChest : MonoBehaviour
{
    [SerializeField] private Image imgCurrency;

    [SerializeField] private Sprite imgWeaponTicket;
    [SerializeField] private Sprite imgSkillTicket;

    private Animator animator;
    private Player player;
    private Vector2 targetPos;

    public Animator Animator => animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        targetPos = new Vector2(0, 3);
    }

    public void SetUp(Player player)
    {
        int num = Random.Range(0, 2);
        if (num == 0)
        {
            imgCurrency.sprite = imgWeaponTicket;
            player.CurrencySystem.IncreaseCurrency(CurrencyType.EquipmentTicket, 10);
        }
        else
        {
            imgCurrency.sprite = imgSkillTicket;
            player.CurrencySystem.IncreaseCurrency(CurrencyType.SkillTicket, 10);
        }

        // UI는 RectTransform을 사용하며 anchoredPosition으로 움직임
        imgCurrency.rectTransform.anchoredPosition = Vector2.zero;
        animator.SetBool(Settings.isChestOpen, true);
    }

    public void ChestOpen() // 애니메이션 이벤트
    {
        imgCurrency.gameObject.SetActive(true);

        imgCurrency.rectTransform.DOAnchorPos(targetPos,2f)
            .SetEase(Ease.OutQuint)
            .OnComplete(ChestClose);
    }

    private void ChestClose()
    {
        animator.SetBool(Settings.isChestOpen, false);
        imgCurrency.gameObject.SetActive(false);
    }
}
