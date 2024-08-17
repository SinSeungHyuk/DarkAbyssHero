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

    [SerializeField] private List<CurrencyUIElement> currencyUIElements = new List<CurrencyUIElement>();

    private Player player;


    private void Start()
    {
        player = GameManager.Instance.GetPlayer();
        player.CurrencySystem.OnCurrencyChanged += UpdateCurrencyUI;
    }

    private void UpdateCurrencyUI(CurrencySystem system, CurrencyType type)
    {
        var txt = currencyUIElements.FirstOrDefault(x => x.type == type).txtCurrency;
        txt.text = player.CurrencySystem.GetCurrency(type).ToString("N0");
    }
}
