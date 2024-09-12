using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ContentsController : MonoBehaviour
{
    [SerializeField] private GameObject modalView; // ���������� ������ UI�޴� Ʋ
    [SerializeField] private GameObject innerContentView; // ���� �������� ������ UI (��ų,����)
    [SerializeField] private TextMeshProUGUI txtModalViewTitle; // UI�޴��� ����
    [SerializeField] private Button btnClose; // �ݱ��ư
    [SerializeField] private List<GameObject> contentsView = new(); // ������UI���� ����ִ� ����Ʈ
    [SerializeField] private ScrollRect scrollView; // �������� ������ ��ũ�Ѻ�

    private Player player;
    private Database stageDB;
    private Database skillDB;
    private Database weaponDB;


    private void Start()
    {
        player = GameManager.Instance.GetPlayer();

        stageDB = AddressableManager.Instance.GetResource<Database>("StageDatabase");
        skillDB = AddressableManager.Instance.GetResource<Database>("SkillDatabase");
        weaponDB = AddressableManager.Instance.GetResource<Database>("WeaponDatabase");
    }

    public void BtnStage()
    {
        BtnContents((int)ContentsType.Stage);

        txtModalViewTitle.text = "Stage";

        // ������UI ���� �������� ��ư�� �����ͼ� ��������DB ������ �Ѱ��ֱ�
        BtnStage[] btnStages = contentsView[(int)ContentsType.Stage].GetComponentsInChildren<BtnStage>();
        for (int i = 0; i < btnStages.Length; i++) 
        {
            btnStages[i].SetUp(player, stageDB.GetDataByID(i) as Stage);
        }
    }

    public void BtnStats()
    {
        BtnContents((int)ContentsType.Stats);

        txtModalViewTitle.text = "Stats";

        StatsUI statsUI = contentsView[(int)ContentsType.Stats].GetComponent<StatsUI>();
        statsUI.SetUp(player);
    }

    public void BtnLevelUp()
    {
        BtnContents((int)ContentsType.LevelUp);

        txtModalViewTitle.text = "Level UP";

        ItemLevelUp[] btnLevelUp = contentsView[(int)ContentsType.LevelUp].GetComponentsInChildren<ItemLevelUp>();
        for (int i = 0; i < btnLevelUp.Length; i++)
        {
            btnLevelUp[i].SetUp(player);
        }
    }

    public void BtnSkill()
    {
        BtnContents((int)ContentsType.Skill);

        txtModalViewTitle.text = "Skill";

        BtnSkill[] btnSkill = contentsView[(int)ContentsType.Skill].GetComponentsInChildren<BtnSkill>();
        for (int i = 1; i < skillDB.Count; i++)
        {
            btnSkill[i - 1].SetUp(player, skillDB.GetDataByID(i) as Skill);
        }
    }

    public void BtnEquipment()
    {
        BtnContents((int)ContentsType.Equipment);

        txtModalViewTitle.text = "Equipment";

        BtnWeapon[] btnSkill = contentsView[(int)ContentsType.Equipment].GetComponentsInChildren<BtnWeapon>();
        for (int i = 0; i < btnSkill.Length; i++)
        {
            btnSkill[i].SetUp(player, weaponDB.GetDataByID(i) as Weapon);
        }
    }

    public void BtnShop()
    {
        contentsView[(int)ContentsType.Shop].GetComponent<ShopUI>().SetUp(player,skillDB,weaponDB);
        contentsView[(int)ContentsType.Shop].gameObject.SetActive(true);
    }


    private void BtnContents(int idx)
    {
        if (contentsView[idx].activeSelf) return; // �̹� Ȱ��ȭ ���̶�� ����

        foreach (var content in contentsView) content.gameObject.SetActive(false); // �ٸ� ������UI ��� �ݱ�

        // ��ũ�Ѻ��� content�� ���� �����ִ� ������UI�� RectTransform���� �����ؾ� ��ũ�Ѻ� �巡�װ� ����
        scrollView.content = contentsView[idx].gameObject.GetComponent<RectTransform>();

        innerContentView.SetActive(false);
        modalView.SetActive(true);
        contentsView[idx].SetActive(true);
        btnClose.gameObject.SetActive(true);
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() => OnBtnClose(idx));
    }

    private void OnBtnClose(int idx)
    {
        contentsView[idx].gameObject.SetActive(false);
        modalView.SetActive(false);
        innerContentView.SetActive(false);
    }
}
