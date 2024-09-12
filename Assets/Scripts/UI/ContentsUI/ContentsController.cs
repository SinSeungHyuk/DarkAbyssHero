using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ContentsController : MonoBehaviour
{
    [SerializeField] private GameObject modalView; // 공통적으로 보여줄 UI메뉴 틀
    [SerializeField] private GameObject innerContentView; // 세부 디테일을 보여줄 UI (스킬,무기)
    [SerializeField] private TextMeshProUGUI txtModalViewTitle; // UI메뉴의 제목
    [SerializeField] private Button btnClose; // 닫기버튼
    [SerializeField] private List<GameObject> contentsView = new(); // 컨텐츠UI들이 들어있는 리스트
    [SerializeField] private ScrollRect scrollView; // 컨텐츠를 보여줄 스크롤뷰

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

        // 컨텐츠UI 안의 스테이지 버튼들 가져와서 스테이지DB 데이터 넘겨주기
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
        if (contentsView[idx].activeSelf) return; // 이미 활성화 중이라면 리턴

        foreach (var content in contentsView) content.gameObject.SetActive(false); // 다른 컨텐츠UI 모두 닫기

        // 스크롤뷰의 content를 현재 보고있는 컨텐츠UI의 RectTransform으로 설정해야 스크롤뷰 드래그가 가능
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
