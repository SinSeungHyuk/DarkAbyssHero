using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region PLAYER PARAMETER
    public static float detectionRadius = 10.0f;
    public static LayerMask monsterLayer = LayerMask.GetMask("Monster");
    public static LayerMask stackingLayer = LayerMask.GetMask("Stacking");

    public static float projectileDistance = 500.0f;
    public static string shootPoint = "ShootPoint";

    public static int startExp = 100;
    public static float expPerLevel = 1.04f; // 레벨당 경험치 4% 증가

    public static int maxLevel = 10;
    #endregion


    #region DRAW PARAMETER
    public static float normalChance = 0.6f; // 노말 60%
    public static float rareChance = 0.3f; // 레어 30%
    public static float epicChance = 0.07f; // 에픽 7%
    public static float legendChance = 0.03f; // 전설 3%
    #endregion


    #region STAGE PARAMETER
    public static float spawnTimerMax = 10.0f;
    public static float spawnTimerMin = 7.0f;

    public static float monsterReadyTimer = 1.0f;

    public static int killsToReward = 10;
    #endregion


    #region Animator Parameter
    public static int LocomotionState = Animator.StringToHash("Locomotion");

    public static int speed = Animator.StringToHash("speed");
    public static int isDead = Animator.StringToHash("isDead");
    public static int isMagicAreaAttack = Animator.StringToHash("isMagicAreaAttack");
    public static int isUpHandCast = Animator.StringToHash("isUpHandCast");
    public static int isClapCast = Animator.StringToHash("isClapCast");
    public static int isStandingShoot = Animator.StringToHash("isStandingShoot");
    public static int isCasting = Animator.StringToHash("isCasting");

    public static int isAttack = Animator.StringToHash("isAttack");
    public static int AttackState = Animator.StringToHash("Attack");

    public static int isChestOpen = Animator.StringToHash("isChestOpen");
    #endregion


    #region GAME SETTING
    public const float musicFadeOutTime = 0.5f;
    public const float musicFadeInTime = 0.5f;

    public static string URL = "https://blog.naver.com/tmdgur0147/223532833451";
    #endregion


    #region Colors
    public static Color32 blue = new Color32(76, 115, 209, 255);
    public static Color32 green = new Color32(22, 135, 24, 255);
    public static Color32 red = new Color32(255, 112, 120, 255);
    public static Color32 beige = new Color32(207, 182, 151, 255);
    public static Color32 rare = new Color32(11, 110, 204, 255);
    public static Color32 epic = new Color32(155, 61, 217, 255);
    public static Color32 legend = new Color32(226, 125, 19, 255);
    public static Color32 critical = new Color32(255, 102, 2, 255);
    #endregion
}
