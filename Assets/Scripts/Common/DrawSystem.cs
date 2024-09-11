using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSystem : MonoBehaviour
{
    // 무기,스킬을 등급별로 분류해놓은 딕셔너리 (등급별 확률 존재)
    private Dictionary<GradeType, List<Weapon>> weaponsByGrade = new Dictionary<GradeType, List<Weapon>>();
    private Dictionary<GradeType, List<Skill>> skillsByGrade = new Dictionary<GradeType, List<Skill>>();
    // 등급별 뽑기 확률이 들어있는 리스트
    private List<float> drawChanceByGrade = new List<float>();


    public void SetUp()
    {
        Database weaponDB = AddressableManager.Instance.GetResource<Database>("WeaponDatabase");
        Database skillDB = AddressableManager.Instance.GetResource<Database>("SkillDatabase");

        for (int i = 0; i < weaponDB.Count; i++)
        {
            Weapon weapon = weaponDB.GetDataByID(i) as Weapon;

            // 딕셔너리에 키값이 비어있으면 빈 리스트 삽입
            if (!weaponsByGrade.TryGetValue(weapon.GradeType, out _))
                weaponsByGrade[weapon.GradeType] = new List<Weapon>();

            weaponsByGrade[weapon.GradeType].Add(weapon);
        }

        for (int i = 0; i < skillDB.Count; i++)
        {
            Skill skill = skillDB.GetDataByID(i) as Skill;

            if (!skillsByGrade.TryGetValue(skill.GradeType, out _))
                skillsByGrade[skill.GradeType] = new List<Skill>();

            skillsByGrade[skill.GradeType].Add(skill);
        }

        // 각 등급별 정해진 확률 리스트에 삽입
        drawChanceByGrade.Add(Settings.normalChance);
        drawChanceByGrade.Add(Settings.rareChance);
        drawChanceByGrade.Add(Settings.epicChance);
        drawChanceByGrade.Add(Settings.legendChance);
    }

    public Weapon DrawWeapon()
    {
        float randomValue = Random.value; // (0 ~ 1)
        float currentChance = 0.0f;

        // 1. 0~1 사이의 랜덤한 수를 뽑는다.
        // 2. 노말부터 순서대로 정해진 확률을 누적하여 더한다. (확률은 0~1 사이의 소수)
        // 3. 난수가 누적확률보다 낮으면 해당 등급을 키값으로 랜덤한 무기를 뽑아 리턴
        for (int i = 0; i < drawChanceByGrade.Count; i++)
        {
            currentChance += drawChanceByGrade[i];
            if (randomValue <= currentChance)
            {
                GradeType grade = (GradeType)i;

                int randomIndex = Random.Range(0, weaponsByGrade[grade].Count);
                return weaponsByGrade[grade][randomIndex];
            }
        }

        return weaponsByGrade[GradeType.Normal][0];
    }

    public Skill DrawSkill()
    {
        float randomValue = Random.value;
        float currentChance = 0.0f;

        for (int i = 0; i < drawChanceByGrade.Count; i++)
        {
            currentChance += drawChanceByGrade[i];
            if (randomValue <= currentChance)
            {
                GradeType grade = (GradeType)i;

                int randomIndex = Random.Range(0, skillsByGrade[grade].Count);
                return skillsByGrade[grade][randomIndex];
            }
        }

        return skillsByGrade[GradeType.Normal][0];
    }
}
