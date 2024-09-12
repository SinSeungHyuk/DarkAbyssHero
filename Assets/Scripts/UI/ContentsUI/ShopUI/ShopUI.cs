using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private DrawViewUI drawViewUI;
    [SerializeField] private TextMeshProUGUI txtWeaponTicket;
    [SerializeField] private TextMeshProUGUI txtSkillTicket;

    private DrawSystem drawSystem;
    private Player player;
    private Database skillDB;
    private Database weaponDB;


    private void Start()
    {
        // 상점창이 활성화되면 최초로 한 번 뽑기시스템을 세팅
        drawSystem = GetComponent<DrawSystem>();
        drawSystem.SetUp();
    }

    void Update()
    {
        // 모바일에서 뒤로가기 버튼 -> KeyCode.Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    public void SetUp(Player player, Database skillDB, Database weaponDB)
    {
        this.player = player;

        txtWeaponTicket.text = player.CurrencySystem.GetCurrency(CurrencyType.EquipmentTicket).ToString();
        txtSkillTicket.text = player.CurrencySystem.GetCurrency(CurrencyType.SkillTicket).ToString();

        this.skillDB = skillDB;
        this.weaponDB = weaponDB;
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

        List<Weapon> drawWeapons = new(count); // 뽑기 결과를 담을 리스트

        for (int i = 0; i < count; i++)
        {
            Weapon weapon = drawSystem.DrawWeapon();
            player.WeaponSystem.RegisterWeapon(weapon); // 뽑은 무기 등록하기
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
