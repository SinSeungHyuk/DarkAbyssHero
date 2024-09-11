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
        yield return new WaitForSeconds(3.0f);

        PlayerRevive();
    }

    public void BtnOK()
    {
        StopCoroutine(AutoRevive());

        PlayerRevive();
    }

    private void PlayerRevive()
    {
        this.gameObject.SetActive(false);

        GameManager.Instance.PlayerRevive();
    }
}
