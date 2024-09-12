using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadUI : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(AutoRevive());
    }

    private IEnumerator AutoRevive()
    {
        // 5초 뒤에 자동으로 부활 (방치형 게임이므로)
        yield return new WaitForSeconds(5.0f);

        PlayerRevive();
    }

    public void BtnOK()
    {
        StopCoroutine(AutoRevive()); // 자동부활 코루틴 중지

        PlayerRevive();
    }

    private void PlayerRevive()
    {
        this.gameObject.SetActive(false);

        GameManager.Instance.PlayerRevive();
    }
}
