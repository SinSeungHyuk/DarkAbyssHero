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
    private Weapon ownWeapon;
    private Player player;

    private string green;
    private string red;


    private void Awake()
    {
        green = ColorUtility.ToHtmlStringRGB(Settings.green);
        red = ColorUtility.ToHtmlStringRGB(Settings.red);
    }

    #region Skill Contents 
    public void SetUp(Player player, Skill skill)
    {
        this.player = player;
        ownSkill = player.SkillSystem.FindOwnSkills(skill);

        txtLevel.text = $"Lv. {ownSkill.Level} / {ownSkill.MaxLevel}";
        txtUpCurrency.text = $"{ownSkill.SkillGrade.GradeCurrency}";

        txtName.text = ownSkill.DisplayName;
        txtName.color = ownSkill.SkillGrade.GradeColor;
        imgIcon.sprite = ownSkill.Icon;
        imgFrame.color = ownSkill.SkillGrade.GradeColor;

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
            && !ownSkill.IsMaxLevel)
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
    #endregion


    #region Weapon Contents
    public void SetUp(Player player, Weapon weapon)
    {
        this.player = player;
        ownWeapon = player.WeaponSystem.FindOwnWeapon(weapon);

        txtLevel.text = $"Lv. {ownWeapon.Level} / {ownWeapon.MaxLevel}";
        txtUpCurrency.text = $"{ownWeapon.WeaponGrade.GradeCurrency}";

        txtName.text = ownWeapon.DisplayName;
        txtName.color = ownWeapon.WeaponGrade.GradeColor;
        imgIcon.sprite = ownWeapon.Icon;
        imgFrame.color = ownWeapon.WeaponGrade.GradeColor;

        btnLevelUp.onClick.RemoveAllListeners();
        btnEquip.onClick.RemoveAllListeners();
        btnLevelUp.onClick.AddListener(() => WeaponLevelUp());
        btnEquip.onClick.AddListener(() => WeaponEquip());
    }

    private void WeaponLevelUp()
    {
        if (player.CurrencySystem.GetCurrency(CurrencyType.EquipmentUp) >= ownWeapon.WeaponGrade.GradeCurrency
            && !ownWeapon.IsMaxLevel)
        {
            player.CurrencySystem.IncreaseCurrency(CurrencyType.EquipmentUp, -ownWeapon.WeaponGrade.GradeCurrency);
            ownWeapon.Level++;
            txtLevel.text = $"Lv. {ownWeapon.Level} / {ownWeapon.MaxLevel}";
            //txtInfo3.text = $"ATK Scailing : <color=#{red}>{ownWeapon.CurrentDatas.}</color>" +
            //    $" -> <color=#{red}>{ownSkill.Effects[0].EffectAction.GetEffectCoefficient(ownSkill.Level + 1)}</color>";
        }
    }

    private void WeaponEquip()
    {
        if (player.WeaponSystem.CurrentWeapon.ID != ownWeapon.ID)
        {
            player.WeaponSystem.UnequipWeapon(player.WeaponSystem.CurrentWeapon);
            player.WeaponSystem.EquipWeapon(ownWeapon);
        }
    }
    #endregion
}
