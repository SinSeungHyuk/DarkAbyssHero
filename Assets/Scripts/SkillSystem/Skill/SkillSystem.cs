using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SkillSystem : MonoBehaviour
{
    public event Action<SkillSystem, Skill> OnSkillEquip;
    public event Action<SkillSystem, Skill> OnSkillUnequip;


    // 임시로 테스트를 위해 직렬화필드로 선언
    [SerializeField] private List<Skill> equipSkills = new(6);
    // 사용가능한 스킬이 없을때 사용할 기본스킬
    [SerializeField] private Skill defaultSkill;
    [SerializeField] private List<Skill> testSkill = new List<Skill>();

    // 소유 중인 스킬리스트 (실제 스킬셋에 장착과는 별개)
    private List<Skill> ownSkills = new();
    
    public IReadOnlyList<Skill> EquipSkills => equipSkills;
    public IReadOnlyList<Skill> OwnSkills => ownSkills;
    public Skill DefaultSkill { get; private set; } // 기본스킬
    public Skill ReserveSkill { get; private set; } // 플레이어가 사용할 예약스킬

    public Player Player { get; private set; }



    private void OnDestroy()
    {
        //foreach (var skill in ownSkills)
        //    Destroy(skill);

        foreach (var skill in equipSkills)
            Destroy(skill);
    }

    public void SetUp(Player player)
    {
        Player = player;

        // 임시코드        
        foreach (var skill in testSkill)
        {
            var clone = skill.Clone() as Skill;
            clone.SetUp(Player, 1);

            equipSkills.Add(clone);
            ownSkills.Add(clone);
        }

        var defaultClone = defaultSkill.Clone() as Skill;
        defaultClone.SetUp(Player);
        DefaultSkill = defaultClone;


        // 스킬 장착이벤트 : UI 스킬셋에 등록
        //OnSkillEquip?.Invoke(this, clone);
    }

    private void Update()
    {
        foreach (var skill in equipSkills)
        {
            // SkillSystem Update -> Skill Update -> StateMachine Update -> State Update
            skill.Update();
        }

        DefaultSkill.Update();
    }

    public void EquipSkill(Skill skill, int level = 1)
    {
        Debug.Assert(!equipSkills.Exists(x => x.ID == skill.ID), "SkillSystem::EquipSkill - 이미 장착한 Skill입니다.");

        var clone = skill.Clone() as Skill;
        if (level > 1)
            clone.SetUp(Player, level);
        else         
            clone.SetUp(Player);

        equipSkills.Add(clone);

        // 스킬 장착이벤트 : UI 스킬셋에 등록
        OnSkillEquip?.Invoke(this, clone);
    }

    public bool UnequipSkill(Skill skill)
    {
        skill = FindEquipSkills(skill);
        if (skill == null)
            return false;

        skill.Cancel();
        equipSkills.Remove(skill);

        // 스킬 해제시 스킬셋 UI에 반영
        OnSkillUnequip?.Invoke(this, skill);

        Destroy(skill);

        return true;
    }

    public void RegisterSkill(Skill skill)
    {
        if (ownSkills.Contains(skill)) return;

        ownSkills.Add(skill);
    }

    public bool FindUsableSkill()
    {
        // 장착중인 스킬리스트에서 IsReady 상태인 스킬
        // 그중에서 우선순위가 높은 순서대로 찾기

        Skill skill = equipSkills.Where(x => x.IsReady)
                          .OrderByDescending(x => x.SkillPriority)
                          .FirstOrDefault();

        if (skill == null) // 장착스킬에서 사용가능한게 없을때
        {
            // 기본스킬이 사용가능한 상태인지 검사
            if (DefaultSkill.IsInState<ReadyState>())
                ReserveSkill = DefaultSkill;
            else return false;
        }
        else
        {
            ReserveSkill = skill;
        }

        Player.Movement.StopDistance = ReserveSkill.Distance;

        return true;
    }

    private void ApplyCurrentRunningSkill()
    {
        if (Player.StateMachine.GetCurrentState() is InSkillActionState ownerState)
        {
            var runnsingSkill = ownerState.RunningSkill;
            if (runnsingSkill.ApplyType != SkillApplyType.Animation) return;

            runnsingSkill.Apply();
        }
    }


    #region Utility
    public Skill FindEquipSkills(Skill skill)
        => equipSkills.Find(x => x.ID == skill.ID);

    public Skill FindOwnSkills(Skill skill)
    => ownSkills.Find(x => x.ID == skill.ID);

    public bool ContainsEquipSkills(Skill skill)
    => FindEquipSkills(skill) != null;

    public bool ContainsOwnSkills(Skill skill)
    => FindOwnSkills(skill) != null;
    #endregion
}
