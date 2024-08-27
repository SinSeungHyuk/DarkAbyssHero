using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnSkill : MonoBehaviour
{
    [SerializeField] private InnerContentUI innerContentView;
    [SerializeField] private TextMeshProUGUI txtEquipped;
    [SerializeField] private TextMeshProUGUI txtSkillLevel;
    [SerializeField] private Image imgSkillIcon;
    [SerializeField] private Image imgFrame;
    [SerializeField] private Image imgBlind;

    private Button btnSkill;
    private Skill skill;
    private Player player;
    private Color32 gradeColor;


    private void Awake()
    {
        btnSkill = GetComponent<Button>();
        btnSkill.onClick.AddListener(OnBtnSkillClick);
    }

    public void SetUp(Player player, Skill skill)
    {
        this.player = player;
        this.skill = skill;

        imgSkillIcon.sprite = skill.Icon;
        ShowSkillGradeColor();

        if (!player.SkillSystem.ContainsOwnSkills(skill))
            imgBlind.gameObject.SetActive(true);
        else
            ShowSkillLevel();
        if (player.SkillSystem.ContainsEquipSkills(skill))
            txtEquipped.gameObject.SetActive(true);
    }


    private void OnBtnSkillClick()
    {
        if (!player.SkillSystem.ContainsOwnSkills(skill))
            return;

        innerContentView.gameObject.SetActive(true);
        innerContentView.SetUp(player,skill);
    }

    private void ShowSkillLevel()
    {
        txtSkillLevel.gameObject.SetActive(true);
        txtSkillLevel.text = $"Lv. {player.SkillSystem.FindOwnSkills(skill).Level}";
    }

    private void ShowSkillGradeColor()
    {
        switch (skill.GradeType)
        {
            case GradeType.Normal:
                imgFrame.color = Color.white;
                gradeColor = Color.white;
                break;
            case GradeType.Rare:
                imgFrame.color = Settings.rare;
                gradeColor = Settings.rare;
                break;
            case GradeType.Epic:
                imgFrame.color = Settings.epic;
                gradeColor = Settings.epic;
                break;
            case GradeType.Legend:
                imgFrame.color = Settings.legend;
                gradeColor = Settings.legend;
                break;
            default:
                break;
        };
    }
}
