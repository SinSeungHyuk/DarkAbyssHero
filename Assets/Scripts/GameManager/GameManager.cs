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

    [SerializeField] private Camera stackingCamera;
    [SerializeField] private RewardChest chest;

    private UniversalAdditionalCameraData cameraData;
    private bool isShowCameraStack = false;
    private bool isChestActive = false;

    public bool IsChestActive => isChestActive;


    private void Start()
    {
        // ����ī�޶��� URP ī�޶� �Ӽ� ��������
        cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Remove(stackingCamera);
    }

    private void Update()
    {
        // isShowCameraStack : ���ʿ��� Ray ����� �����ϱ� ����
        if (Input.GetMouseButtonUp(0) && isShowCameraStack)
        {
            Ray ray = stackingCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Settings.stackingLayer))
            {
                // ���ʷ� �ѹ��� Ŭ���� �� �ְԲ�
                isShowCameraStack = false;

                chest.SetUp(player);

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

        chest.Animator.Play("Chest_Start", -1, 0f);
    }

    public void GiveReward(double timeStamp)
    {
        if (timeStamp < 1) return; // 1�ð� �̸��� �������� X
        if (timeStamp > 12) timeStamp = 12; // 12�ð� ������ ��������

        (int,int)goldExpRewards = StageManager.Instance.CurrentStage.GetAvgRewards((int)timeStamp);

        player.CurrencySystem.IncreaseCurrency(CurrencyType.Gold, goldExpRewards.Item1);
        player.LevelSystem.GetExpReward(goldExpRewards.Item2);

        uiController.SetUpRewardUI(goldExpRewards.Item1, goldExpRewards.Item2, (int)timeStamp);
    }



    public Player GetPlayer() => player;
}
