using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    [SerializeField] private RewardUI rewardUI;
    [SerializeField] private OptionUI optionUI;
    [SerializeField] private HelpUI helpUI;
    [SerializeField] private DeadUI deadUI;
    [SerializeField] private Image imgFade;


    private void Start()
    {
        StageManager.Instance.OnStageChanged += Instance_OnStageChanged;
    }

    private void Instance_OnStageChanged(Stage stage, int level)
    {
        imgFade.gameObject.SetActive(true);

        // ���̵� �̹����� ������ 1�� �����صα� (���������ٰ� �ٽ� �������ϰ� �����)
        Color color = imgFade.color;
        color.a = 1;
        imgFade.color = color;

        // DOFade : �̹����� ��⸦ 2�ʿ� ���ļ� 0���� (���̵� �ƿ�)
        imgFade.DOFade(0f, 2f).OnComplete(() => { imgFade.gameObject.SetActive(false); });
    }

    public void SetUpRewardUI(int gold, int exp, int hours)
    {
        rewardUI.SetUp(gold, exp, hours);
        rewardUI.gameObject.SetActive(true);
    }

    public void BtnHelp()
    {
        helpUI.gameObject.SetActive(true);
    }

    public void BtnOption()
    {
        optionUI.gameObject.SetActive(true);
    }

    public void SetUpDeadUI()
    {
        deadUI.gameObject.SetActive(true);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
