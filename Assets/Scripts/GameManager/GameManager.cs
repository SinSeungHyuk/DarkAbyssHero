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
using UnityEngine.Rendering.Universal;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIController uiController;
    [SerializeField] private Player player;

    // 스테이지 보상을 위한 URP의 카메라 스태킹 활용
    [SerializeField] private Camera stackingCamera;
    [SerializeField] private RewardChest chest; 

    private UniversalAdditionalCameraData cameraData;
    private bool isShowCameraStack = false;
    private bool isChestActive = false;

    public bool IsChestActive => isChestActive;


    private void Start()
    {
        // 메인카메라의 URP 카메라 속성 가져오기
        cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Remove(stackingCamera);
    }

    private void Update()
    {
        // isShowCameraStack : 불필요한 Ray 계산을 생략하기 위함
        if (Input.GetMouseButtonUp(0) && isShowCameraStack)
        {
            Ray ray = stackingCamera.ScreenPointToRay(Input.mousePosition); // 터치한 위치
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Settings.stackingLayer))
            {
                // 최초로 한번만 클릭할 수 있게끔
                isShowCameraStack = false;

                chest.SetUp(player);

                //  DOVirtual.DelayedCall : 3.5초 뒤에 람다식 호출
                DOVirtual.DelayedCall(3.5f, () => {
                    cameraData.cameraStack.Remove(stackingCamera);
                    isChestActive = false;
                });
            }
        }
    }

    public void SetRewardChest()
    {
        isChestActive = true;
        isShowCameraStack = true;
        cameraData.cameraStack.Add(stackingCamera);

        // 보상상자가 등장할때마다 다시 첫 애니메이션으로 돌아가서 재생
        chest.Animator.Play("Chest_Start", -1, 0f);
    }

    public void GiveReward(double timeStamp)
    {
        if (timeStamp < 1) return; // 1시간 미만은 보상지급 X
        if (timeStamp > 12) timeStamp = 12; // 12시간 까지만 보상지급

        (int,int)goldExpRewards = StageManager.Instance.CurrentStage.GetAvgRewards((int)timeStamp);

        player.CurrencySystem.IncreaseCurrency(CurrencyType.Gold, goldExpRewards.Item1);
        player.LevelSystem.GetExpReward(goldExpRewards.Item2);

        uiController.SetUpRewardUI(goldExpRewards.Item1, goldExpRewards.Item2, (int)timeStamp);
    }

    public void PlayerDead()
    {
        uiController.SetUpDeadUI();
    }

    public void PlayerRevive()
    {
        // 마지막으로 죽은 스테이지를 다시 새롭게 생성
        Stage stage = StageManager.Instance.CurrentStage;
        StageManager.Instance.CreateStage(stage);

        player.OnRevive();
    }


    public Player GetPlayer() => player;
}
