using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    private SkillSystem skillSystem;
    private List<SkillSlot> slots = new(6);


    private void Awake()
    {
        slots = GetComponentsInChildren<SkillSlot>().ToList();
    }

    private void Start()
    {
        Player player = GameManager.Instance.GetPlayer();
        skillSystem = player.SkillSystem;

        // 처음 0번 자리에 기본스킬 등록
        slots[0].Skill = skillSystem.EquipSkills[0];

        skillSystem.OnSkillEquip += OnSkillEquip;
    }

    private void OnSkillEquip(SkillSystem system, Skill skill, int idx)
    {
        slots[idx].Skill = skill;
    }
}
