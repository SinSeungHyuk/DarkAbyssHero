using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawViewUI : MonoBehaviour
{
    [SerializeField] private Transform drawResultTransform;
    [SerializeField] private ItemDrawResult itemDrawResult;

    // 뽑기결과 차례대로 보여주기
    private WaitForSeconds _wait = new WaitForSeconds(0.15f);


    public void SetUp(List<Weapon> drawWeapons)
    {
        StartCoroutine(InstantiateDrawRoutine(drawWeapons));
    }

    public void SetUp(List<Skill> drawSkills)
    {
        StartCoroutine(InstantiateDrawRoutine(drawSkills));
    }

    private IEnumerator InstantiateDrawRoutine(List<Skill> drawSkills)
    {
        // 미리 랜덤으로 뽑은 결과가 담긴 리스트를 순회하며 UI로 보여주기
        foreach (var skill in drawSkills)
        {
            itemDrawResult.SetUp(skill);
            Instantiate(itemDrawResult, drawResultTransform);

            yield return _wait;
        }
    }

    private IEnumerator InstantiateDrawRoutine(List<Weapon> drawWeapons)
    {
        foreach (var weapon in drawWeapons)
        {
            itemDrawResult.SetUp(weapon);
            Instantiate(itemDrawResult, drawResultTransform);

            yield return _wait;
        }
    }

    public void BtnConfirm()
    {
        // Transform을 순회하면 직계자식들을 가져올 수 있음
        foreach (Transform item in drawResultTransform)
        {
            Destroy(item.gameObject);
        }

        gameObject.SetActive(false);
    }
}
