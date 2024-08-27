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


    // �ӽ÷� �׽�Ʈ�� ���� ����ȭ�ʵ�� ����
    [SerializeField] private List<Skill> equipSkills = new(6);
    // ��밡���� ��ų�� ������ ����� �⺻��ų
    [SerializeField] private Skill defaultSkill;
    [SerializeField] private List<Skill> testSkill = new List<Skill>();

    // ���� ���� ��ų����Ʈ (���� ��ų�¿� �������� ����)
    private List<Skill> ownSkills = new();
    
    public IReadOnlyList<Skill> EquipSkills => equipSkills;
    public IReadOnlyList<Skill> OwnSkills => ownSkills;
    public Skill DefaultSkill { get; private set; } // �⺻��ų
    public Skill ReserveSkill { get; private set; } // �÷��̾ ����� ���ེų

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

        // �ӽ��ڵ�        
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


        // ��ų �����̺�Ʈ : UI ��ų�¿� ���
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
        Debug.Assert(!equipSkills.Exists(x => x.ID == skill.ID), "SkillSystem::EquipSkill - �̹� ������ Skill�Դϴ�.");

        var clone = skill.Clone() as Skill;
        if (level > 1)
            clone.SetUp(Player, level);
        else         
            clone.SetUp(Player);

        equipSkills.Add(clone);

        // ��ų �����̺�Ʈ : UI ��ų�¿� ���
        OnSkillEquip?.Invoke(this, clone);
    }

    public bool UnequipSkill(Skill skill)
    {
        skill = FindEquipSkills(skill);
        if (skill == null)
            return false;

        skill.Cancel();
        equipSkills.Remove(skill);

        // ��ų ������ ��ų�� UI�� �ݿ�
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
        // �������� ��ų����Ʈ���� IsReady ������ ��ų
        // ���߿��� �켱������ ���� ������� ã��

        Skill skill = equipSkills.Where(x => x.IsReady)
                          .OrderByDescending(x => x.SkillPriority)
                          .FirstOrDefault();

        if (skill == null) // ������ų���� ��밡���Ѱ� ������
        {
            // �⺻��ų�� ��밡���� �������� �˻�
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
