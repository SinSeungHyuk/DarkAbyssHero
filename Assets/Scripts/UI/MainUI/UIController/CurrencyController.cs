using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class CurrencyController : UIController
{
    [Serializable]
    public struct CurrencyUIElement
    {
        public CurrencyType type;
        public TextMeshProUGUI txtCurrency;
    }

    // UI 여기저기 흩어져있는 화폐텍스트를 등록해서 각각 화폐의 종류까지 설정
    [SerializeField] private List<CurrencyUIElement> currencyUIElements = new List<CurrencyUIElement>();

    private Player player;


    private void Start()
    {
        player = GameManager.Instance.GetPlayer();
        player.CurrencySystem.OnCurrencyChanged += UpdateCurrencyUI;

        SetStartCurrency();
    }

    private void SetStartCurrency()
    {
        var txt = currencyUIElements.FirstOrDefault(x => x.type == CurrencyType.Gold).txtCurrency;
        txt.text = player.CurrencySystem.GetCurrency(CurrencyType.Gold).ToString("N0");
    }

    private void UpdateCurrencyUI(CurrencySystem system, CurrencyType type)
    {
        var txt = currencyUIElements.FirstOrDefault(x => x.type == type).txtCurrency;
        txt.text = player.CurrencySystem.GetCurrency(type).ToString("N0");
    }
}
