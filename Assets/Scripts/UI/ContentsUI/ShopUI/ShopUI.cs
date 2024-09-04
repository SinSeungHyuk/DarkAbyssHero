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
        if (player.CurrencySystem.GetCurrency(CurrencyType.EquipmentTicket) < 10)
            return;

        player.CurrencySystem.IncreaseCurrency(CurrencyType.EquipmentTicket, -10);

        List<Weapon> drawWeapons = new();

        for (int i = 0; i < 10; i++)
        {
            Weapon weapon = drawSystem.DrawWeapon();
            drawWeapons.Add(weapon);
        }

        drawViewUI.gameObject.SetActive(true);
        drawViewUI.SetUp(drawWeapons);
    }

    public void BtnWeaponDraw30()
    {
        if (player.CurrencySystem.GetCurrency(CurrencyType.EquipmentTicket) < 30)
            return;

        player.CurrencySystem.IncreaseCurrency(CurrencyType.EquipmentTicket, -30);

        List<Weapon> drawWeapons = new(30);

        for (int i = 0; i < 30; i++)
        {
            Weapon weapon = drawSystem.DrawWeapon();
            drawWeapons.Add(weapon);
        }

        drawViewUI.gameObject.SetActive(true);
        drawViewUI.SetUp(drawWeapons);
    }
}
