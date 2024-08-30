using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{


    [SerializeField] private Weapon defaultWeapon; // �⺻����
    [SerializeField] private Weapon testWeapon; // �׽�Ʈ ����

    // ���� ���� ��ų����Ʈ (���� ��ų�¿� �������� ����)
    private List<Weapon> ownWeapons = new();

    public IReadOnlyList<Weapon> OwnWeapons => ownWeapons;
    public Weapon CurrentWeapon { get; private set; } // �������� ����

    public Player Player { get; private set; }


    public void SetUp(Player player)
    {
        Player = player;

        var defaultClone = defaultWeapon.Clone() as Weapon;
        defaultClone.SetUp(Player, 10);

        var testWeapons = testWeapon.Clone() as Weapon;
        testWeapons.SetUp(Player, 1);
        ownWeapons.Add(defaultClone);
        ownWeapons.Add(testWeapons);

        EquipWeapon(defaultClone);
    }

    public void EquipWeapon(Weapon weapon)
    {
        foreach (WeaponData weaponData in weapon.CurrentDatas)
        {
            Player.Stats.IncreaseBonusValue(weaponData.Stat, weapon, weaponData.BonusStatValue);
        }
    }

    public bool UnequipWeapon(Weapon weapon)
    {
        

        return true;
    }
}
