using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    //                      스테이지 레벨
    public event Action<Stage, int> OnStageChanged;

    private Database stageDB;
    private GameObject currentStagePrefab = null;
    private Stage currentStage;
    
    public Stage CurrentStage => currentStage;



    protected override void Awake()
    {
        base.Awake();

        stageDB = AddressableManager.Instance.GetResource<Database>("StageDatabase");
    }

    public void CreateStage(Stage stage)
    {
        if (currentStagePrefab != null) Destroy(currentStagePrefab);

        currentStage = stage;
        currentStagePrefab = Instantiate(stage.StagePrefab, transform);

        // 현재 스테이지와 레벨을 넘겨서 이벤트 호출
        OnStageChanged?.Invoke(currentStage, currentStage.StageLevel);
    }

    public void CreateStage(int stageID)
    {
        Stage stage = stageDB.GetDataByID(0) as Stage;
        CreateStage(stage);
    }
}
