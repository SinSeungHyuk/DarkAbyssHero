using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    [SerializeField] private SkillSlot slotPrefab;

    private SkillSystem skillSystem;
    private List<SkillSlot> slots = new();
    private int emptySlotIndex;

    private void Start()
    {
        Player player = GameManager.Instance.GetPlayer();
        skillSystem = player.SkillSystem;
        //skillSystem.OnSkillRegistered += OnSkillRegistered;

        var ownSkills = skillSystem.EquipSkills;

            slotPrefab.Skill = ownSkills[0];
            slots.Add(slotPrefab);
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
