using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BtnEquipSkills : MonoBehaviour
{
    private List<BtnEquipSkill> slots = new(6);
    Player player;
    Skill ownSkill;


    private void Awake()
    {
        slots = GetComponentsInChildren<BtnEquipSkill>().ToList();

        for (int i = 0; i < slots.Count; i++)
        {
            int index = i; // 람다식 캡처를 위해 지역변수 사용
            slots[i].GetComponent<Button>().onClick.AddListener(
                () => EquipSkill(index));
        }
    }

    public void SetUp(Player player, Skill ownSkill)
    {
        this.player = player;
        this.ownSkill = ownSkill;

        for (int i = 0; i < slots.Count; i++)
        {
            Skill skill = player.SkillSystem.EquipSkills[i];
            slots[i].SetUp(skill);
        }
    }

    private void EquipSkill(int idx)
    {
        player.SkillSystem.EquipSkill(ownSkill, idx);
        gameObject.SetActive(false);
    }
}
