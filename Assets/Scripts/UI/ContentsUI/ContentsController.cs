using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentsController : MonoBehaviour
{
    [SerializeField] private GameObject modalView; // 공통적으로 보여줄 UI메뉴 틀
    [SerializeField] private TextMeshProUGUI txtModalViewTitle; // UI메뉴의 제목
    [SerializeField] private Button btnClose; // 닫기버튼
    [SerializeField] private List<GameObject> contentsView = new(); // 컨텐츠UI들이 들어있는 리스트

    private Player player;
    private Database stageDB;
    private Database statDB;
    private Database skillDB;
    //private Database weaponDB;


    private void Start()
    {
        player = GameManager.Instance.GetPlayer();

        stageDB = AddressableManager.Instance.GetResource<Database>("StageDatabase");
        statDB = AddressableManager.Instance.GetResource<Database>("StatDatabase");
        skillDB = AddressableManager.Instance.GetResource<Database>("SkillDatabase");
    }

    public void BtnStage()
    {
        BtnContents((int)ContentsType.Stage);

        txtModalViewTitle.text = "Stage";

        BtnStage[] btnStages = contentsView[(int)ContentsType.Stage].GetComponentsInChildren<BtnStage>();
        for (int i = 0; i < btnStages.Length; i++) 
        {
                btnStages[i].SetUp(player, stageDB.GetDataByID(i) as Stage);
        }
    }

    private void BtnContents(int idx)
    {
        if (contentsView[idx].activeSelf) return;

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
    }
}
