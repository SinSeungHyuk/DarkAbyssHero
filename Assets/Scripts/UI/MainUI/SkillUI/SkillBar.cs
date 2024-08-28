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

        // ó�� 0�� �ڸ��� �⺻��ų ���
        slots[0].Skill = skillSystem.EquipSkills[0];

        skillSystem.OnSkillEquip += OnSkillEquip;
    }

    private void OnSkillEquip(SkillSystem system, Skill skill, int idx)
    {
        slots[idx].Skill = skill;
    }
}
