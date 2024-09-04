using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawViewUI : MonoBehaviour
{
    [SerializeField] private Transform drawResultTransform;
    [SerializeField] private ItemDrawResult itemDrawResult;


    public void SetUp(List<Weapon> drawWeapons)
    {
        foreach (var weapon in drawWeapons)
        {
            itemDrawResult.SetUp(weapon);
            Instantiate(itemDrawResult, drawResultTransform);
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
