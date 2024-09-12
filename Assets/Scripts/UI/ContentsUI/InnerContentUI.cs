using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Sprite imgSkillCurrency;
    [SerializeField] private Sprite imgWeaponCurrency;
    [SerializeField] private Button btnLevelUp;
    [SerializeField] private Button btnEquip;

    private Skill ownSkill;
    private Weapon ownWeapon;
    private Player player;

    private string green;
    private string red;


    private void Awake()
    {
        // 미리 정의해놓은 색상값 string 타입으로 변환
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

        txtInfo1.text = "";
        txtInfo2.text = "";
        txtInfo3.text = "";

        txtName.text = ownSkill.DisplayName;
        txtName.color = ownSkill.SkillGrade.GradeColor;
        imgIcon.sprite = ownSkill.Icon;
        imgFrame.color = ownSkill.SkillGrade.GradeColor;
        imgCurrency.sprite = imgSkillCurrency;

        txtInfo1.text = $"Cooldown : <color=#{green}>{ownSkill.Cooldown}</color>";
        txtInfo2.text = skill.Description;
        ShowSkillInfoText();

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
            ShowSkillInfoText();
        }
    }

    private void SkillEquip()
    {
        // 이미 장착중인 스킬은 리턴
        if (player.SkillSystem.ContainsEquipSkills(ownSkill)) return;

        btnEquipSkills.gameObject.SetActive(true);
        btnEquipSkills.SetUp(player, ownSkill);
    }

    private void ShowSkillInfoText()
    {
        // 현재 스킬의 계수를 가져오는 함수를 호출하여 "지금 레벨의 계수 -> 다음 레벨의 계수" 출력

        txtInfo3.text = $"ATK Scailing : <color=#{red}>{ownSkill.Effects[0].EffectAction.GetEffectCoefficient(ownSkill.Level)}</color>%" +
    $" -> <color=#{red}>{ownSkill.Effects[0].EffectAction.GetEffectCoefficient(ownSkill.Level + 1)}</color>%";
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
        imgCurrency.sprite = imgWeaponCurrency;

        ShowWeaponInfoText(ownWeapon);

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
            ShowWeaponInfoText(ownWeapon);
        }
    }

    private void WeaponEquip()
    {
        if (player.WeaponSystem.CurrentWeapon.ID != ownWeapon.ID)
        {
            player.WeaponSystem.EquipWeapon(ownWeapon);
        }
    }

    private void ShowWeaponInfoText(Weapon weapon)
    {
        txtInfo1.text = "";
        txtInfo2.text = "";
        txtInfo3.text = "";
        foreach (WeaponData data in weapon.CurrentDatas)
        {
            txtInfo1.text += $"{data.Stat.DisplayName} -> {data.BonusStatValue}\n";
        }
    }
    #endregion
}
