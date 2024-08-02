using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region PLAYER PARAMETER
    
    #endregion

    #region Animator Parameter
    public static int speed = Animator.StringToHash("speed");
    public static int isDead = Animator.StringToHash("isDead");
    public static int isMagicAreaAttack = Animator.StringToHash("isMagicAreaAttack");
    public static int isUpHandCast = Animator.StringToHash("isUpHandCast");
    public static int isClapCast = Animator.StringToHash("isClapCast");
    public static int isStandingShoot = Animator.StringToHash("isStandingShoot");
    #endregion
}
