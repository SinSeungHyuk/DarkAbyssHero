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

    // 소유 중인 무기리스트
    private List<Weapon> ownWeapons = new();

    public IReadOnlyList<Weapon> OwnWeapons => ownWeapons;
    public Weapon CurrentWeapon { get; private set; } // 장착중인 무기

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
        
        // 무기 레벨업 이벤트 구독,해지해서 데이터 실시간으로 가져오기
        CurrentWeapon = equipWeapon;

        CurrentWeapon.OnLevelChanged += CurrentWeapon_OnLevelChanged;
    }

    public bool UnequipWeapon(Weapon weapon)
    {
        CurrentWeapon.OnLevelChanged -= CurrentWeapon_OnLevelChanged;

        // 무기의 보너스 스탯 모두 되돌리기
        foreach (WeaponData weaponData in weapon.CurrentDatas)
        {
            Player.Stats.RemoveBonusValue(weaponData.Stat, weapon);
        }
        OnWeaponUnequiped?.Invoke(this, weapon);

        return true;
    }    

    private void ApplyWeaponDatas(Weapon weapon)
    {
        // 무기가 가진 스탯보너스 모두 적용하기
        foreach (WeaponData weaponData in weapon.CurrentDatas)
        {
            Player.Stats.IncreaseBonusValue(weaponData.Stat, weapon, weaponData.BonusStatValue);
        }
    }

    public void RegisterWeapon(Weapon weapon, int level = 1)
    {
        // 이미 같은 무기를 소유하고 있으면 무기업그레이드 재료 획득
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
        // 소유한 무기, 장착한 무기를 저장해서 반환
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
