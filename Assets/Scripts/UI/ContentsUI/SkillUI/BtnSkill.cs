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

        if (!player.SkillSystem.ContainsOwnSkills(skill)) // 스킬을 보유하고있지않으면 블라인드 처리
            imgBlind.gameObject.SetActive(true);
        else
        {
            Skill findSkill = player.SkillSystem.FindOwnSkills(skill);
            findSkill.OnLevelChanged -= FindSkill_OnLevelChanged; // 중복구독 방지
            findSkill.OnLevelChanged += FindSkill_OnLevelChanged;
            imgBlind.gameObject.SetActive(false);
            ShowSkillLevel();
        }
        if (player.SkillSystem.ContainsEquipSkills(skill)) // 스킬 장착표시
            txtEquipped.gameObject.SetActive(true);
        else txtEquipped.gameObject.SetActive(false);

        player.SkillSystem.OnSkillEquip -= OnEquip;
        player.SkillSystem.OnSkillUnequip -= OnUnequip;
        player.SkillSystem.OnSkillEquip += OnEquip;
        player.SkillSystem.OnSkillUnequip += OnUnequip;
    }

    private void FindSkill_OnLevelChanged(Skill arg1, int arg2, int arg3)
        => ShowSkillLevel(); // 스킬의 레벨이 변할때마다 레벨텍스트 업데이트

    private void OnUnequip(SkillSystem system, Skill skill, int arg3)
    {
        if (skill.ID == this.skill.ID)
            txtEquipped.gameObject.SetActive(false);
    }

    private void OnEquip(SkillSystem system, Skill skill, int arg3)
    {
        if (skill.ID == this.skill.ID)
            txtEquipped.gameObject.SetActive(true);
    }


    private void OnBtnSkillClick()
    {
        if (!player.SkillSystem.ContainsOwnSkills(skill))
            return;

        // 스킬의 자세한 디테일 살펴보기
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
        Color32 color = UtilitieHelper.GetGradeColor(skill.GradeType);

        imgFrame.color = color;
    }
}
