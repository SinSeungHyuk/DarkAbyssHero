using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawViewUI : MonoBehaviour
{
    [SerializeField] private Transform drawResultTransform;
    [SerializeField] private ItemDrawResult itemDrawResult;

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
        foreach (Transform item in drawResultTransform)
        {
            Destroy(item.gameObject);
        }

        gameObject.SetActive(false);
    }
}
