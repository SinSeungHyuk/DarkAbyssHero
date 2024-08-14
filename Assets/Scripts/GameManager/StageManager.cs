using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public event Action<Stage, int> OnStageChanged;

    private GameObject currentStagePrefab = null;
    private Stage currentStage;
    

    public void CreateStage(Stage stage)
    {
        if (currentStagePrefab != null) Destroy(currentStagePrefab);

        currentStage = stage;
        currentStagePrefab = Instantiate(stage.StagePrefab, transform);

        OnStageChanged?.Invoke(currentStage, currentStage.StageLevel);
    }
}
