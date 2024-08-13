using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region PLAYER PARAMETER
    public static float detectionRadius = 15.0f;
    public static LayerMask monsterLayer = LayerMask.GetMask("Monster");

    public static float projectileDistance = 500.0f; 
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
    #endregion


    #region Colors
    public static Color32 blue = new Color32(76, 115, 209, 255);
    public static Color32 green = new Color32(22, 135, 24, 255);
    public static Color32 red = new Color32(255, 112, 120, 255);
    public static Color32 rare = new Color32(11, 110, 204, 255);
    public static Color32 epic = new Color32(155, 61, 217, 255);
    public static Color32 legend = new Color32(226, 125, 19, 255);
    public static Color32 critical = new Color32(255, 102, 2, 255);
    #endregion
}
