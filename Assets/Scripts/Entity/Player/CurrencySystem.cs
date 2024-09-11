using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class CurrencySystem : MonoBehaviour, ISaveData<CurrencySaveData>
{   //                                   화폐종류
    public event Action<CurrencySystem, CurrencyType> OnCurrencyChanged;

    // 화폐 종류는 5개로 고정, 배열은 사이즈넣고 초기화하면 인덱싱 바로 가능
    private int[] currencyList = new int[5]; 


    public Player Player { get; private set; }
    public IReadOnlyList<int> CurrencyList => currencyList;


    public void SetUp(Player player)
    {
        Player = player;
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


    // 저장된 리스트를 받아와 배열로 변환하고 그대로 대입해주어서 로드
    public CurrencySaveData ToSaveData()
        => new CurrencySaveData() { currencyList = currencyList.ToList() };
    public void FromSaveData(CurrencySaveData saveData)
        => currencyList = saveData.currencyList.ToArray();
}

[Serializable]
public struct CurrencySaveData
{
    public List<int> currencyList;
}