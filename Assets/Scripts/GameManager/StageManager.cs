using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    //                      스테이지 레벨
    public event Action<Stage, int> OnStageChanged;

    private GameObject currentStagePrefab = null;
    private Stage currentStage;
    
    public Stage CurrentStage => currentStage;



    public void CreateStage(Stage stage)
    {
        if (currentStagePrefab != null) Destroy(currentStagePrefab);

        currentStage = stage;
        currentStagePrefab = Instantiate(stage.StagePrefab, transform);

        // 현재 스테이지와 레벨을 넘겨서 이벤트 호출
        OnStageChanged?.Invoke(currentStage, currentStage.StageLevel);
    }
}
