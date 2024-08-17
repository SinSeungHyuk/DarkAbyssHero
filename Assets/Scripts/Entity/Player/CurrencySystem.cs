using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class CurrencySystem : MonoBehaviour, ISaveData<CurrencySaveData>
{   //                                   È­ÆóÁ¾·ù
    public event Action<CurrencySystem, CurrencyType> OnCurrencyChanged;

    private int[] currencyList;


    public Player Player { get; private set; }
    public IReadOnlyList<int> CurrencyList => currencyList;


    public void SetUp(Player player)
    {
        Player = player;

        currencyList = new int[5];
    }

    public void IncreaseCurrency(CurrencyType type, int amount)
    {
        currencyList[Convert.ToInt32(type)] += amount;
        OnCurrencyChanged?.Invoke(this, type);
    }

    public int GetCurrency(int type)
        => currencyList[type];
    public int GetCurrency(CurrencyType type)
        => currencyList[Convert.ToInt32(type)];


    public CurrencySaveData ToSaveData()
        => new CurrencySaveData() { currencyList = currencyList };
    public void FromSaveData(CurrencySaveData saveData)
        => currencyList = saveData.currencyList;
}

[Serializable]
public struct CurrencySaveData
{
    public int[] currencyList;
}