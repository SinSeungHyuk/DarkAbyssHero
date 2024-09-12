using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnStage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtStageName;
    [SerializeField] private TextMeshProUGUI txtStageLevel;
    [SerializeField] private Button btnEnter;
    [SerializeField] private Image imgFrame;

    private int stageRequiredLevel;
    private Stage stage;
    private Player player;


    public void SetUp(Player player, Stage stage)
    {
        stageRequiredLevel = stage.StageRequiredLevel;
        this.stage = stage;
        this.player = player;

        txtStageName.text = stage.DisplayName;
        txtStageLevel.text = "Level : "+ stageRequiredLevel.ToString();

        if (StageManager.Instance.CurrentStage == stage) // 현재 이 스테이지라면 버튼 비활성화
        {
            btnEnter.interactable = false;
            imgFrame.gameObject.SetActive(true);
        }
        if (player.LevelSystem.Level < stageRequiredLevel) // 스테이지의 입장레벨 조건
            btnEnter.interactable = false;

        player.LevelSystem.OnLevelChanged += OnLevelChanged;
        StageManager.Instance.OnStageChanged += OnStageChanged;
        btnEnter.onClick.AddListener(() =>
            {
                StageManager.Instance.CreateStage(stage);
            });
    }

    private void OnDisable()
    {
        player.LevelSystem.OnLevelChanged -= OnLevelChanged;
        StageManager.Instance.OnStageChanged -= OnStageChanged;
        btnEnter.onClick.RemoveAllListeners();
    }

    private void OnLevelChanged(LevelSystem system, int level)
    {
        if (level >= stageRequiredLevel && StageManager.Instance.CurrentStage != stage)
            btnEnter.interactable = true;
    }

    private void OnStageChanged(Stage stage, int level)
    {
        // 스테이지를 변경한 순간 이 스테이지로 변경했는지, 레벨은 부합한지 등 검사

        if (stage == this.stage)
        {
            btnEnter.interactable = false;
            imgFrame.gameObject.SetActive(true);
        }
        else
        {
            if (player.LevelSystem.Level < stageRequiredLevel)
                btnEnter.interactable = false;
            else btnEnter.interactable = true;
            imgFrame.gameObject.SetActive(false);
        }
    }
}
