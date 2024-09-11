using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSystem : MonoBehaviour
{
    // ����,��ų�� ��޺��� �з��س��� ��ųʸ� (��޺� Ȯ�� ����)
    private Dictionary<GradeType, List<Weapon>> weaponsByGrade = new Dictionary<GradeType, List<Weapon>>();
    private Dictionary<GradeType, List<Skill>> skillsByGrade = new Dictionary<GradeType, List<Skill>>();
    // ��޺� �̱� Ȯ���� ����ִ� ����Ʈ
    private List<float> drawChanceByGrade = new List<float>();


    public void SetUp()
    {
        Database weaponDB = AddressableManager.Instance.GetResource<Database>("WeaponDatabase");
        Database skillDB = AddressableManager.Instance.GetResource<Database>("SkillDatabase");

        for (int i = 0; i < weaponDB.Count; i++)
        {
            Weapon weapon = weaponDB.GetDataByID(i) as Weapon;

            // ��ųʸ��� Ű���� ��������� �� ����Ʈ ����
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

        // �� ��޺� ������ Ȯ�� ����Ʈ�� ����
        drawChanceByGrade.Add(Settings.normalChance);
        drawChanceByGrade.Add(Settings.rareChance);
        drawChanceByGrade.Add(Settings.epicChance);
        drawChanceByGrade.Add(Settings.legendChance);
    }

    public Weapon DrawWeapon()
    {
        float randomValue = Random.value; // (0 ~ 1)
        float currentChance = 0.0f;

        // 1. 0~1 ������ ������ ���� �̴´�.
        // 2. �븻���� ������� ������ Ȯ���� �����Ͽ� ���Ѵ�. (Ȯ���� 0~1 ������ �Ҽ�)
        // 3. ������ ����Ȯ������ ������ �ش� ����� Ű������ ������ ���⸦ �̾� ����
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
