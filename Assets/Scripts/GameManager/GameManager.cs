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
    [SerializeField] private UIController uiController;
    [SerializeField] private Player player;


    public void GiveReward(double timeStamp)
    {
        if (timeStamp < 1) return; // 1시간 미만은 보상지급 X
        if (timeStamp > 12) timeStamp = 12; // 12시간 까지만 보상지급

        (int,int)goldExpRewards = StageManager.Instance.CurrentStage.GetAvgRewards((int)timeStamp);

        player.CurrencySystem.IncreaseCurrency(CurrencyType.Gold, goldExpRewards.Item1);
        player.LevelSystem.GetExpReward(goldExpRewards.Item2);

        uiController.SetUpRewardUI(goldExpRewards.Item1, goldExpRewards.Item2, (int)timeStamp);
    }



    public Player GetPlayer() => player;
}
