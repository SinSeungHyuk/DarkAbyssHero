using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Player player;
    [SerializeField] private Database stageDB;



    private void Start()
    {
        Stage stage = stageDB.GetDataByID(0) as Stage;

        StageManager.Instance.CreateStage(stage);
    }


    public Player GetPlayer() => player;
}
