using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public event Action<WeaponSystem, Weapon> OnWeaponEquiped;
    public event Action<WeaponSystem, Weapon> OnWeaponUnequiped;

    [SerializeField] private Weapon defaultWeapon; // 기본무기
    [SerializeField] private Weapon testWeapon; // 테스트 무기

    // 소유 중인 스킬리스트 (실제 스킬셋에 장착과는 별개)
    private List<Weapon> ownWeapons = new();

    public IReadOnlyList<Weapon> OwnWeapons => ownWeapons;
    public Weapon CurrentWeapon { get; private set; } // 장착중인 무기

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
            CurrentWeapon = weapon;
            OnWeaponEquiped?.Invoke(this, weapon);
            // 무기 레벨업 이벤트 구독,해지해서 데이터 실시간으로 가져오기
        }
    }

    public bool UnequipWeapon(Weapon weapon)
    {
        foreach (WeaponData weaponData in weapon.CurrentDatas)
        {
            Player.Stats.RemoveBonusValue(weaponData.Stat, weapon);
            OnWeaponUnequiped?.Invoke(this, weapon);
        }

        return true;
    }


    #region Utility
    public Weapon FindOwnWeapon(Weapon weapon)
    => ownWeapons.Find(x => x.ID == weapon.ID);

    public bool ContainsOwnWeapons(Weapon weapon)
    => FindOwnWeapon(weapon) != null;
    #endregion
}
