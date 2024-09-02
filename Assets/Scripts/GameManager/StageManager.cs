using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    //                      �������� ����
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

        // ���� ���������� ������ �Ѱܼ� �̺�Ʈ ȣ��
        OnStageChanged?.Invoke(currentStage, currentStage.StageLevel);
    }

    public void CreateStage(int stageID)
    {
        Stage stage = stageDB.GetDataByID(0) as Stage;
        CreateStage(stage);
    }
}
