using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;
using Firebase.Database;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private TextMeshProUGUI txtTest;
    [SerializeField] private Player player;

    private Database stageDB;


    protected override void Awake()
    {
        base.Awake();

        stageDB = AddressableManager.Instance.GetResource<Database>("StageDatabase");
    }

    private void Start()
    {
        //Stage stage = stageDB.GetDataByID(0) as Stage;
        //StageManager.Instance.CreateStage(stage);

        
    }

    public void GiveReward(double timeStamp)
    {
        txtTest.text = timeStamp.ToString();
        Debug.Log(timeStamp + " GiveRewardGiveRewardGiveRewardGiveRewardGiveRewardGiveRewardGiveRewardGiveRewardGiveRewardGiveRewardGiveReward");
    }



    public Player GetPlayer() => player;
}
