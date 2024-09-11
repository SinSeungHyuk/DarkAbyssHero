using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class CurrencySystem : MonoBehaviour, ISaveData<CurrencySaveData>
{   //                                   ȭ������
    public event Action<CurrencySystem, CurrencyType> OnCurrencyChanged;

    // ȭ�� ������ 5���� ����, �迭�� ������ְ� �ʱ�ȭ�ϸ� �ε��� �ٷ� ����
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


    // ����� ����Ʈ�� �޾ƿ� �迭�� ��ȯ�ϰ� �״�� �������־ �ε�
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