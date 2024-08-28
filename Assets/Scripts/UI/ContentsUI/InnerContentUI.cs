using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InnerContentUI : MonoBehaviour
{
    [SerializeField] private BtnEquipSkills btnEquipSkills;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtInfo1;
    [SerializeField] private TextMeshProUGUI txtInfo2;
    [SerializeField] private TextMeshProUGUI txtInfo3;
    [SerializeField] private TextMeshProUGUI txtUpCurrency;
    [SerializeField] private Image imgIcon;
    [SerializeField] private Image imgFrame;
    [SerializeField] private Image imgCurrency;
    [SerializeField] private Button btnLevelUp;
    [SerializeField] private Button btnEquip;

    private Skill ownSkill;
    private Player player;

    private string green;
    private string red;


    private void Awake()
    {
        green = ColorUtility.ToHtmlStringRGB(Settings.green);
        red = ColorUtility.ToHtmlStringRGB(Settings.red);
    }

    public void SetUp(Player player, Skill skill)
    {
        this.player = player;
        ownSkill = player.SkillSystem.FindOwnSkills(skill);

        txtLevel.text = $"Lv. {ownSkill.Level} / {ownSkill.MaxLevel}";
        txtUpCurrency.text = $"{ownSkill.SkillGrade.GradeCurrency}";
        ShowInfo();

        txtInfo1.text = $"Cooldown : <color=#{green}>{ownSkill.Cooldown}</color> sec";
        txtInfo2.text = skill.Description;
        txtInfo3.text = $"ATK Scailing : <color=#{red}>{ownSkill.Effects[0].EffectAction.GetEffectCoefficient(ownSkill.Level)}</color>" +
            $" -> <color=#{red}>{ownSkill.Effects[0].EffectAction.GetEffectCoefficient(ownSkill.Level+1)}</color>";

        btnLevelUp.onClick.RemoveAllListeners();
        btnEquip.onClick.RemoveAllListeners();
        btnLevelUp.onClick.AddListener(() => SkillLevelUp());
        btnEquip.onClick.AddListener(() => SkillEquip());
    }

    private void SkillLevelUp()
    {
        if (player.CurrencySystem.GetCurrency(CurrencyType.SkillUp) >= ownSkill.SkillGrade.GradeCurrency
            && ownSkill.MaxLevel > ownSkill.Level)
        {
            player.CurrencySystem.IncreaseCurrency(CurrencyType.SkillUp, -ownSkill.SkillGrade.GradeCurrency);
            ownSkill.Level++;
            txtLevel.text = $"Lv. {ownSkill.Level} / {ownSkill.MaxLevel}";
            txtInfo3.text = $"ATK Scailing : <color=#{red}>{ownSkill.Effects[0].EffectAction.GetEffectCoefficient(ownSkill.Level)}</color>" +
                $" -> <color=#{red}>{ownSkill.Effects[0].EffectAction.GetEffectCoefficient(ownSkill.Level + 1)}</color>";
        }
    }

    private void SkillEquip()
    {
        // 이미 장착중인 스킬은 리턴
        if (player.SkillSystem.ContainsEquipSkills(ownSkill)) return;

        btnEquipSkills.gameObject.SetActive(true);
        btnEquipSkills.SetUp(player, ownSkill);
    }


    //public void SetUp(Weapon weapon)
    //{

    //}

    private void ShowInfo()
    {
        txtName.text = ownSkill.DisplayName;
        txtName.color = ownSkill.SkillGrade.GradeColor;
        imgIcon.sprite = ownSkill.Icon;
        imgFrame.color = ownSkill.SkillGrade.GradeColor;
    }
}
