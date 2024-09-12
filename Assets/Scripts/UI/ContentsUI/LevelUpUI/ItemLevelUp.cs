using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemLevelUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtInfo;
    [SerializeField] private TextMeshProUGUI txtGold;
    [SerializeField] private BtnLevelUp btnLevelUp;
    [SerializeField] private StatType statType;

    private Stat stat;
    private Player player;


    public void SetUp(Player player)
    {
        this.player = player;
        this.stat = player.Stats.GetStat(statType);

        ShowStatInfo();
        IsStatRequiredLevel();

        stat.OnLevelChanged += OnLevelChanged;
        player.LevelSystem.OnLevelChanged += OnPlayerLevelChanged;
        btnLevelUp.SetUp(player,stat);
    }

    private void OnDisable()
    {
        stat.OnLevelChanged -= OnLevelChanged;
        player.LevelSystem.OnLevelChanged += OnPlayerLevelChanged;
    }

    private void OnLevelChanged(Stat stat, float level)
    {
        // 각 스탯의 레벨이 변할때마다 텍스트 업데이트
        ShowStatInfo(); 
    }

    private void OnPlayerLevelChanged(LevelSystem system, int level)
    {
        // 스탯을 레벨업하기 위한 플레이어 레벨 검사
        IsStatRequiredLevel();
    }

    private void ShowStatInfo()
    {
        if (stat.ID == (int)StatType.HP)
            txtInfo.text = $"Level : {stat.Level}\n" +
                $"{stat.MaxValue} -> {(stat.MaxValue) + (stat.ValuePerLevel)}";
        else
            txtInfo.text = $"Level : {stat.Level}\n" +
                $"{stat.DefaultValue} -> {(stat.DefaultValue) + (stat.ValuePerLevel)}";

        txtGold.text = stat.CurrentGold.ToString("N0");
    }

    private void IsStatRequiredLevel()
    {
        if (player.LevelSystem.Level < stat.RequiredLevel)
        {
            txtInfo.color = Settings.red;
            txtInfo.text = $"Required Level : {stat.RequiredLevel}";
            btnLevelUp.GetComponent<Button>().interactable = false;
        }
        else
        {
            txtInfo.color = Settings.beige;
            ShowStatInfo();
            btnLevelUp.GetComponent<Button>().interactable = true;
        }
    }
}
