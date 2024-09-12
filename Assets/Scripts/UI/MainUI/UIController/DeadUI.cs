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
        // 5�� �ڿ� �ڵ����� ��Ȱ (��ġ�� �����̹Ƿ�)
        yield return new WaitForSeconds(5.0f);

        PlayerRevive();
    }

    public void BtnOK()
    {
        StopCoroutine(AutoRevive()); // �ڵ���Ȱ �ڷ�ƾ ����

        PlayerRevive();
    }

    private void PlayerRevive()
    {
        this.gameObject.SetActive(false);

        GameManager.Instance.PlayerRevive();
    }
}
