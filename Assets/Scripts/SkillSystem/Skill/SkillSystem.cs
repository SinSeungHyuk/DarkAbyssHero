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


    // ��밡���� ��ų�� ������ ����� �⺻��ų
    [SerializeField] private Skill defaultSkill;
    // ó�� ���ӽ� �⺻������ �־����� ��ų
    [SerializeField] private Skill firstSkill;

    // ���� ���� ��ų����Ʈ (���� ��ų�¿� �������� ����)
    private List<Skill> ownSkills = new();
    // ������ ��ų���Կ� ������ ��ų����Ʈ
    private List<Skill> equipSkills = new(6);

    public IReadOnlyList<Skill> EquipSkills => equipSkills;
    public IReadOnlyList<Skill> OwnSkills => ownSkills;
    public Skill DefaultSkill { get; private set; } // �⺻��ų
    public Skill ReserveSkill { get; private set; } // �÷��̾ ����� ���ེų

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
            // ������ų ����Ʈ�� �ε����ϱ� ���� null ����
            equipSkills.Add(null);
        }

        Player = player;

        // SetUp������ �켱 �⺻��ų�� ���� (���̺갡 ������ ����� ��ų����)
        RegisterSkill(firstSkill);          
        EquipSkill(firstSkill, 0);

        // �⺻��ų�� ���� ����Ʈ�� �����ʰ� ���� ����
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
        // ������ų ����Ʈ�� Ư�� �ε����� �޾Ƽ� �ش� �ε����� ����

        Skill equipSkill = FindOwnSkills(skill);
        if (equipSkill == null) return;

        if (equipSkills[idx] != null)
            UnequipSkill(equipSkills[idx], idx);
        equipSkills[idx] = equipSkill;

        //// ��ų �����̺�Ʈ : UI ��ų�¿� ���
        OnSkillEquip?.Invoke(this, equipSkill, idx);
    }

    public bool UnequipSkill(Skill skill, int idx)
    {
        skill = FindEquipSkills(skill);
        if (skill == null) return false;

        skill.Cancel();
        equipSkills.Remove(skill);

        // ��ų ������ ��ų�� UI�� �ݿ�
        OnSkillUnequip?.Invoke(this, skill, idx);

        return true;
    }

    public void RegisterSkill(Skill skill, int level = 1)
    {
        // �̹� ������ ��ų�̸� ��޿� ���� ��ų ���׷��̵� ��� ȹ��
        if (ContainsOwnSkills(skill))
        {
            int currency = UtilitieHelper.GetGradeCurrency(skill.GradeType);
            Player.CurrencySystem.IncreaseCurrency(CurrencyType.SkillUp, currency);
            return;
        }

        // �������� ��ų�� ���� ������ �����ϹǷ� �����ؾ���
        var clone = skill.Clone() as Skill;
        clone.SetUp(Player, level);
        ownSkills.Add(clone);

        OnSkillRegister?.Invoke(this, clone);
    }

    public bool FindUsableSkill()
    {
        // �������� ��ų����Ʈ���� IsReady ������ ��ų
        // ���߿��� �켱������ ���� ������� ã��

        Skill skill = equipSkills.Where(x => x != null && x.IsReady)
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

    // ��ų�� �ִϸ��̼� �̺�Ʈ (�ִϸ��̼� Ư�� Ÿ�ֿ̹� ��ų�ߵ�)
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
        // ������ ��ų, ������ ��ų ����Ʈ�� ���� �����ؼ� ��ȯ
        // ������ų ����Ʈ�� null�� ������ �� �����Ƿ� Where���� �߰�
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