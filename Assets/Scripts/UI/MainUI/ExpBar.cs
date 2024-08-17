using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] private Image imgBar;
    [SerializeField] private TextMeshProUGUI txtRatio;
    private Player player;


    private void Start()
    {
        player = GameManager.Instance.GetPlayer();

        player.LevelSystem.OnExpChanged += UpdateExpBar;
    }

    private void UpdateExpBar(LevelSystem system, float exp, float expLevel)
    {
        imgBar.fillAmount = exp / expLevel;
        txtRatio.text = $"{(exp / expLevel * 100):0} %";
    }
}
