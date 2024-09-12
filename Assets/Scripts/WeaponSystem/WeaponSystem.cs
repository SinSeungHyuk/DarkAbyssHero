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

    // ���� ���� ���⸮��Ʈ
    private List<Weapon> ownWeapons = new();

    public IReadOnlyList<Weapon> OwnWeapons => ownWeapons;
    public Weapon CurrentWeapon { get; private set; } // �������� ����

    public Player Player { get; private set; }


    public void SetUp(Player player)
    {
        Player = player;

        var defaultClone = defaultWeapon.Clone() as Weapon;
        defaultClone.SetUp(Player, 1);

        ownWeapons.Add(defaultClone);

        EquipWeapon(defaultClone);
    }

    public void EquipWeapon(Weapon weapon)
    {
        Weapon equipWeapon = FindOwnWeapon(weapon);
        if (equipWeapon == null) return;

        if (CurrentWeapon != null)
            UnequipWeapon(CurrentWeapon);

        ApplyWeaponDatas(equipWeapon);
        OnWeaponEquiped?.Invoke(this, equipWeapon);
        
        // ���� ������ �̺�Ʈ ����,�����ؼ� ������ �ǽð����� ��������
        CurrentWeapon = equipWeapon;

        CurrentWeapon.OnLevelChanged += CurrentWeapon_OnLevelChanged;
    }

    public bool UnequipWeapon(Weapon weapon)
    {
        CurrentWeapon.OnLevelChanged -= CurrentWeapon_OnLevelChanged;

        // ������ ���ʽ� ���� ��� �ǵ�����
        foreach (WeaponData weaponData in weapon.CurrentDatas)
        {
            Player.Stats.RemoveBonusValue(weaponData.Stat, weapon);
        }
        OnWeaponUnequiped?.Invoke(this, weapon);

        return true;
    }    

    private void ApplyWeaponDatas(Weapon weapon)
    {
        // ���Ⱑ ���� ���Ⱥ��ʽ� ��� �����ϱ�
        foreach (WeaponData weaponData in weapon.CurrentDatas)
        {
            Player.Stats.IncreaseBonusValue(weaponData.Stat, weapon, weaponData.BonusStatValue);
        }
    }

    public void RegisterWeapon(Weapon weapon, int level = 1)
    {
        // �̹� ���� ���⸦ �����ϰ� ������ ������׷��̵� ��� ȹ��
        if (ContainsOwnWeapons(weapon))
        {
            int currency = UtilitieHelper.GetGradeCurrency(weapon.GradeType);
            Player.CurrencySystem.IncreaseCurrency(CurrencyType.EquipmentUp, currency);
            return;
        }

        Weapon registerWeapon = weapon.Clone() as Weapon;
        registerWeapon.SetUp(Player, level);
        ownWeapons.Add(registerWeapon);
    }

    private void CurrentWeapon_OnLevelChanged(Weapon weapon, int currentLevel, int prevLevel)
        => ApplyWeaponDatas(weapon);


    #region Utility
    public Weapon FindOwnWeapon(Weapon weapon)
    => ownWeapons.Find(x => x.ID == weapon.ID);

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

        Weapon equipWeapon = weaponDB.GetDataByID(weaponDatas.CurrentWeaponData.id) as Weapon;

        EquipWeapon(equipWeapon);
    }
    #endregion
}

[Serializable]
public struct WeaponSaveDatas
{
    public List<WeaponSaveData> OwnWeaponsData;
    public WeaponSaveData CurrentWeaponData;
}
