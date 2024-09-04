using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDrawResult : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Image imgFrame;


    public void SetUp(Weapon weapon)
    {
        Color32 color = UtilitieHelper.GetGradeColor(weapon.GradeType);
        imgFrame.color = color;

        imgIcon.sprite = weapon.Icon;
    }

    public void SetUp(Skill skill)
    {
        Color32 color = UtilitieHelper.GetGradeColor(skill.GradeType);
        imgFrame.color = color;

        imgIcon.sprite = skill.Icon;
    }
}
