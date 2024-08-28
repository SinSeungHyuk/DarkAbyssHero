using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnEquipSkill : MonoBehaviour
{
    [SerializeField] private Image imgSkillIcon;


    public void SetUp(Skill skill)
    {
        if (skill == null) return;

        imgSkillIcon.sprite = skill.Icon;
    }
}
