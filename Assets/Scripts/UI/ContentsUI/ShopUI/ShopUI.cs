using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private DrawViewUI drawViewUI;

    private DrawSystem drawSystem;
    private Player player;
    private Database skillDB;
    private Database weaponDB;


    private void Start()
    {
        drawSystem = GetComponent<DrawSystem>();
        drawSystem.SetUp();
    }

    public void SetUp(Player player, Database skillDB, Database weaponDB)
    {
        this.player = player;

        this.skillDB = skillDB;
        this.weaponDB = weaponDB;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    public void BtnWeaponDraw10()
    {
        DrawWeapon(10);
    }

    public void BtnWeaponDraw30()
    {
        DrawWeapon(30);
    }

    public void BtnSkillDraw10()
    {
        DrawSkill(10);
    }

    public void BtnSkillDraw30()
    {
        DrawSkill(30);
    }

    private void DrawWeapon(int count)
    {
        if (player.CurrencySystem.GetCurrency(CurrencyType.EquipmentTicket) < count)
            return;

        player.CurrencySystem.IncreaseCurrency(CurrencyType.EquipmentTicket, -count);

        List<Weapon> drawWeapons = new(count);

        for (int i = 0; i < count; i++)
        {
            Weapon weapon = drawSystem.DrawWeapon();
            player.WeaponSystem.RegisterWeapon(weapon);
            drawWeapons.Add(weapon);
        }

        drawViewUI.gameObject.SetActive(true);
        drawViewUI.SetUp(drawWeapons);
    }

    private void DrawSkill(int count)
    {
        if (player.CurrencySystem.GetCurrency(CurrencyType.SkillTicket) < count)
            return;

        player.CurrencySystem.IncreaseCurrency(CurrencyType.SkillTicket, -count);

        List<Skill> drawSkills = new(count);

        for (int i = 0; i < count; i++)
        {
            Skill skill = drawSystem.DrawSkill();
            player.SkillSystem.RegisterSkill(skill);
            drawSkills.Add(skill);
        }

        drawViewUI.gameObject.SetActive(true);
        drawViewUI.SetUp(drawSkills);
    }
}
