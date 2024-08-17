using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Image imgBar;
    [SerializeField] private TextMeshProUGUI txtRatio;
    private Player player;


    private void Start()
    {
        player = GameManager.Instance.GetPlayer();

        player.DamageEvent.OnTakeDamage += UpdateHPBar;
    }

    private void UpdateHPBar(DamageEvent @event, TakeDamageEventArgs args)
    {
        float ratio = player.Stats.GetHPStatRatio();

        imgBar.fillAmount = ratio;
        txtRatio.text = $"{player.Stats.HPStat.DefaultValue.ToString("0")}" +
            $" / {player.Stats.HPStat.MaxValue.ToString("0")}";
    }
}
