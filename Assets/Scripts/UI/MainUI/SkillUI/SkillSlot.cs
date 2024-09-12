using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using System;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private Image imgSkill;
    [SerializeField] GameObject blindObject;
    [SerializeField] Image imgBlind;
    [SerializeField] TextMeshProUGUI txtCooldown;

    private Skill skill;

    public Skill Skill
    {
        get => skill;
        set
        {
            if (skill) // 이미 스킬이 등록된 슬롯이면 기존 구독 해제
                skill.OnStateChanged -= OnSkillStateChanged;

            skill = value;

            if (skill != null)
            {
                skill.OnStateChanged += OnSkillStateChanged;

                imgSkill.gameObject.SetActive(true);
                imgSkill.sprite = skill.Icon;
            }
        }
    }


    private void Awake()
    {
        SetSkillUIActive(false);
    }

    private void SetSkillUIActive(bool isOn)
    {
        blindObject.gameObject.SetActive(isOn);
    }

    private void OnSkillStateChanged(Skill skill, State<Skill> nowState, State<Skill> prevState)
    {
        var stateType = nowState.GetType();

        // 스킬슬롯의 상태가 쿨다운 상태라면 코루틴 시작
        if (stateType == typeof(CooldownState))
            StartCoroutine(ShowCooldown());
    }

    private IEnumerator ShowCooldown()
    {
        blindObject.gameObject.SetActive(true);

        while (skill.IsInState<CooldownState>())
        {
            txtCooldown.text = skill.CurrentCooldown.ToString("0");
            imgBlind.fillAmount = skill.CurrentCooldown / skill.Cooldown;
            yield return null;
        }

        imgBlind.fillAmount = 1f;
        blindObject.gameObject.SetActive(false);
    }

}