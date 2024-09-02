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
            CurrentWeapon = weapon;
            OnWeaponEquiped?.Invoke(this, weapon);
            // ���� ������ �̺�Ʈ ����,�����ؼ� ������ �ǽð����� ��������
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

    public void RegisterWeapon(Weapon weapon, int level = 1)
    {
        Weapon registerWeapon = weapon.Clone() as Weapon;
        registerWeapon.SetUp(Player, level);
        ownWeapons.Add(registerWeapon);
    }


    #region Utility
    public Weapon FindOwnWeapon(Weapon weapon)
    => ownWeapons.Find(x => x.ID == weapon.ID);

    public Weapon FindOwnWeapon(int id)
    => ownWeapons.Find(x => x.ID == id);

    public bool ContainsOwnWeapons(Weapon weapon)
    => FindOwnWeapon(weapon) != null;
    #endregion


    #region Weapon Save/Load
    public WeaponSaveDatas ToSaveData()
    {
        // ������ ����, ������ ���⸦ �����ؼ� ��ȯ
        var saveData = new WeaponSaveDatas();
        saveData.OwnWeaponsData = ownWeapons.Select(x => x.ToSaveData()).ToList();
        saveData.CurrentWeaponData = CurrentWeapon.ToSaveData();

        return saveData;
    }

    public void FromSaveData(WeaponSaveDatas weaponDatas)
    {
        Database weaponDB = AddressableManager.Instance.GetResource<Database>("WeaponDatabase");

        ownWeapons.Clear();

        weaponDatas.OwnWeaponsData.ForEach(data =>
            RegisterWeapon(weaponDB.GetDataByID(data.id) as Weapon, data.level));

        EquipWeapon(FindOwnWeapon(weaponDatas.CurrentWeaponData.id));
    }
    #endregion
}

[Serializable]
public struct WeaponSaveDatas
{
    public List<WeaponSaveData> OwnWeaponsData;
    public WeaponSaveData CurrentWeaponData;
}
