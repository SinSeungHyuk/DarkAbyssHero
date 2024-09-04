using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BtnWeapon : MonoBehaviour
{
    [SerializeField] private InnerContentUI innerContentView;
    [SerializeField] private TextMeshProUGUI txtEquipped;
    [SerializeField] private TextMeshProUGUI txtWeaponLevel;
    [SerializeField] private Image imgWeaponIcon;
    [SerializeField] private Image imgFrame;
    [SerializeField] private Image imgBlind;

    private Button btnWeapon;
    private Weapon weapon;
    private Player player;


    private void Awake()
    {
        btnWeapon = GetComponent<Button>();
        btnWeapon.onClick.AddListener(OnBtnWeaponClick);
    }

    public void SetUp(Player player, Weapon weapon)
    {
        this.player = player;
        this.weapon = weapon;

        imgWeaponIcon.sprite = weapon.Icon;
        ShowWeaponGradeColor();

        if (player.WeaponSystem.ContainsOwnWeapons(weapon) == false)
            imgBlind.gameObject.SetActive(true);
        else
        {
            imgBlind.gameObject.SetActive(false);
            ShowWeaponLevel();
        }
        if (player.WeaponSystem.CurrentWeapon.ID == weapon.ID)
            txtEquipped.gameObject.SetActive(true);

        player.WeaponSystem.OnWeaponEquiped -= OnEquip;
        player.WeaponSystem.OnWeaponEquiped += OnEquip;
        player.WeaponSystem.OnWeaponUnequiped -= OnUnequip;
        player.WeaponSystem.OnWeaponUnequiped += OnUnequip;
    }

    private void OnUnequip(WeaponSystem system, Weapon weapon)
    {
        if (weapon.ID == this.weapon.ID)
            txtEquipped.gameObject.SetActive(false);
    }

    private void OnEquip(WeaponSystem system, Weapon weapon)
    {
        if (weapon.ID == this.weapon.ID)
            txtEquipped.gameObject.SetActive(true);
    }

    private void OnBtnWeaponClick()
    {
        if (player.WeaponSystem.ContainsOwnWeapons(weapon) == false)
            return;

        innerContentView.gameObject.SetActive(true);
        innerContentView.SetUp(player, weapon);
    }

    private void ShowWeaponLevel()
    {
        txtWeaponLevel.gameObject.SetActive(true);
        txtWeaponLevel.text = $"Lv. {player.WeaponSystem.FindOwnWeapon(weapon).Level}";
    }

    private void ShowWeaponGradeColor()
    {
        Color32 color = UtilitieHelper.GetGradeColor(weapon.GradeType);

        imgFrame.color = color;
    }
}
