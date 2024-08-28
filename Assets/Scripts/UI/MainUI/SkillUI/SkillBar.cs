using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    private SkillSystem skillSystem;
    private List<SkillSlot> slots = new(6);
    private int emptySlotIndex;


    private void Awake()
    {
        slots = GetComponentsInChildren<SkillSlot>().ToList();
    }

    private void Start()
    {


        Player player = GameManager.Instance.GetPlayer();
        skillSystem = player.SkillSystem;
        slots[3].Skill = skillSystem.EquipSkills[0];
        //skillSystem.OnSkillRegistered += OnSkillRegistered;

        //var ownSkills = skillSystem.EquipSkills;

        //    slotPrefab.Skill = ownSkills[0];
        //    slots.Add(slotPrefab);
    }

    //private void OnDestroy() => skillSystem.onSkillRegistered -= OnSkillRegistered;


    //private void TryAddToEmptySlot(Skill skill)
    //{
    //    if (emptySlotIndex >= slotCount || skill.IsPassive)
    //        return;

    //    slots[emptySlotIndex++].Skill = skill;
    //}

    //private void OnSkillRegistered(SkillSystem skillSystem, Skill skill)
    //    => TryAddToEmptySlot(skill);
}
