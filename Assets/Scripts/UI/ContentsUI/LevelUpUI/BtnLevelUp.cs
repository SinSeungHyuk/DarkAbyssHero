using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnLevelUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float repeatInterval = 0.2f; // �ݺ� ȣ�� ���� (��)
    private bool isPressed = false; // ��ư�� ���� �ִ��� ����

    private Button btnLevelUp;
    private Player player;
    private Stat stat;


    private void Awake()
    {
        btnLevelUp = GetComponent<Button>();
    }

    public void SetUp(Player player, Stat stat)
    {
        this.stat = stat;
        this.player = player;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        StartCoroutine(LevelUpRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    private IEnumerator LevelUpRoutine()
    {
        while (isPressed)
        {
            if (player.CurrencySystem.GetCurrency(CurrencyType.Gold) >= stat.CurrentGold) {
                player.CurrencySystem.IncreaseCurrency(CurrencyType.Gold, -stat.CurrentGold);
                stat.Level++;
            }

            yield return new WaitForSeconds(repeatInterval);
        }
    }
}
