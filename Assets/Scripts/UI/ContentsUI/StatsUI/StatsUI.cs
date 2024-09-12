using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtNickname;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtATK;
    [SerializeField] private TextMeshProUGUI txtMaxHP;
    [SerializeField] private TextMeshProUGUI txtCritChance;
    [SerializeField] private TextMeshProUGUI txtCritDMG;
    [SerializeField] private TextMeshProUGUI txtHPRegen;
    [SerializeField] private TextMeshProUGUI txtHPRegenOnKill;
    [SerializeField] private TextMeshProUGUI txtSpeed;

    private Player player;
    private Stats stats;


    public void SetUp(Player player)
    {
        this.player = player;
        this.stats = player.Stats;

        // 로그인할때 로컬에 저장해놓은 닉네임 가져오기
        txtNickname.text = PlayerPrefs.GetString("Nickname");
        txtLevel.text = $"Level : {player.LevelSystem.Level.ToString()}";

        txtATK.text = stats.GetValue(StatType.Attack).ToString("0");
        txtMaxHP.text = stats.GetStat(StatType.HP).MaxValue.ToString("0"); // HP는 maxHP 출력
        txtCritChance.text = stats.GetValue(StatType.CriticChance).ToString()+ "%";
        txtCritDMG.text = (stats.GetValue(StatType.CriticDamage)*100).ToString("0") + "%";
        txtHPRegen.text = stats.GetValue(StatType.HPRegen).ToString("0");
        txtHPRegenOnKill.text = stats.GetValue(StatType.HPOnKill).ToString("0");
        txtSpeed.text = player.Movement.Speed.ToString("0");

        player.LevelSystem.OnLevelChanged += OnLevelTextChanged;
    }

    private void OnDisable()
    {
        player.LevelSystem.OnLevelChanged -= OnLevelTextChanged;
    }

    private void OnLevelTextChanged(LevelSystem system, int level)
    {
        txtLevel.text = $"Level : {level.ToString()}";
    }
}