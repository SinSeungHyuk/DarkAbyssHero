using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

[RequireComponent(typeof(Player))]
public class SkillSystem : MonoBehaviour
{   //                                     index
    public event Action<SkillSystem, Skill, int> OnSkillEquip;
    public event Action<SkillSystem, Skill, int> OnSkillUnequip;
    public event Action<SkillSystem, Skill> OnSkillRegister;


    // 사용가능한 스킬이 없을때 사용할 기본스킬
    [SerializeField] private Skill defaultSkill;
    // 처음 접속시 기본적으로 주어지는 스킬
    [SerializeField] private Skill firstSkill;

    // 소유 중인 스킬리스트 (실제 스킬셋에 장착과는 별개)
    private List<Skill> ownSkills = new();
    // 실제로 스킬슬롯에 장착한 스킬리스트
    private List<Skill> equipSkills = new(6);

    public IReadOnlyList<Skill> EquipSkills => equipSkills;
    public IReadOnlyList<Skill> OwnSkills => ownSkills;
    public Skill DefaultSkill { get; private set; } // 기본스킬
    public Skill ReserveSkill { get; private set; } // 플레이어가 사용할 예약스킬

    public Player Player { get; private set; }



    private void OnDestroy()
    {
        foreach (var skill in ownSkills)
            Destroy(skill);

        foreach (var skill in equipSkills)
            Destroy(skill);
    }

    public void SetUp(Player player)
    {
        for (int i = 0; i < 6; i++)
        {
            // 장착스킬 리스트에 인덱싱하기 위해 null 삽입
            equipSkills.Add(null);
        }

        Player = player;

        // SetUp에서는 우선 기본스킬을 장착 (세이브가 있으면 저장된 스킬장착)
        RegisterSkill(firstSkill);          
        EquipSkill(firstSkill, 0);

        // 기본스킬은 따로 리스트에 넣지않고 따로 관리
        var defaultClone = defaultSkill.Clone() as Skill;
        defaultClone.SetUp(Player,1);
        DefaultSkill = defaultClone;
    }

    private void Update()
    {
        foreach (var skill in equipSkills)
        {
            // SkillSystem Update -> Skill Update -> StateMachine Update -> State Update
            if (skill == null) continue;
            skill.Update();
        }

        DefaultSkill.Update();
    }

    public void EquipSkill(Skill skill, int idx)
    {
        // 장착스킬 리스트의 특정 인덱스를 받아서 해당 인덱스에 장착

        Skill equipSkill = FindOwnSkills(skill);
        if (equipSkill == null) return;

        if (equipSkills[idx] != null)
            UnequipSkill(equipSkills[idx], idx);
        equipSkills[idx] = equipSkill;

        //// 스킬 장착이벤트 : UI 스킬셋에 등록
        OnSkillEquip?.Invoke(this, equipSkill, idx);
    }

    public bool UnequipSkill(Skill skill, int idx)
    {
        skill = FindEquipSkills(skill);
        if (skill == null) return false;

        skill.Cancel();
        equipSkills.Remove(skill);

        // 스킬 해제시 스킬셋 UI에 반영
        OnSkillUnequip?.Invoke(this, skill, idx);

        return true;
    }

    public void RegisterSkill(Skill skill, int level = 1)
    {
        // 이미 보유한 스킬이면 등급에 따라 스킬 업그레이드 재료 획득
        if (ContainsOwnSkills(skill))
        {
            int currency = UtilitieHelper.GetGradeCurrency(skill.GradeType);
            Player.CurrencySystem.IncreaseCurrency(CurrencyType.SkillUp, currency);
            return;
        }

        // 보유중인 스킬도 각자 레벨을 관리하므로 복제해야함
        var clone = skill.Clone() as Skill;
        clone.SetUp(Player, level);
        ownSkills.Add(clone);

        OnSkillRegister?.Invoke(this, clone);
    }

    public bool FindUsableSkill()
    {
        // 장착중인 스킬리스트에서 IsReady 상태인 스킬
        // 그중에서 우선순위가 높은 순서대로 찾기

        Skill skill = equipSkills.Where(x => x != null && x.IsReady)
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

    // 스킬의 애니메이션 이벤트 (애니메이션 특정 타이밍에 스킬발동)
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
        => equipSkills.Find(x => x != null && x.ID == skill.ID);

    public Skill FindOwnSkills(Skill skill)
    => ownSkills.Find(x => x.ID == skill.ID);

    public bool ContainsEquipSkills(Skill skill)
    => FindEquipSkills(skill) != null;

    public bool ContainsOwnSkills(Skill skill)
    => FindOwnSkills(skill) != null;
    #endregion


    #region Skill Save/Load
    public SkillSaveDatas ToSaveData()
    {
        // 소유한 스킬, 장착한 스킬 리스트를 각각 저장해서 반환
        // 장착스킬 리스트는 null도 존재할 수 있으므로 Where절을 추가
        var saveData = new SkillSaveDatas();
        saveData.EquipSkillsData = equipSkills.Where(x => x != null).Select(x => x.ToSaveData()).ToList();
        saveData.OwnSkillsData = ownSkills.Select(x => x.ToSaveData()).ToList();

        return saveData;
    }

    public void FromSaveData(SkillSaveDatas skillDatas)
    {
        Database skillDB = AddressableManager.Instance.GetResource<Database>("SkillDatabase");

        ownSkills.Clear();

        skillDatas.OwnSkillsData.ForEach(data =>
            RegisterSkill(skillDB.GetDataByID(data.id) as Skill, data.level));

        for (int i = 0; i < skillDatas.EquipSkillsData.Count; i++)
        {
            Skill equipSkill = skillDB.GetDataByID(skillDatas.EquipSkillsData[i].id) as Skill;
            EquipSkill(equipSkill, i);
        }
    }
    #endregion
}

[Serializable]
public struct SkillSaveDatas
{
    public List<SkillSaveData> EquipSkillsData;
    public List<SkillSaveData> OwnSkillsData;
}