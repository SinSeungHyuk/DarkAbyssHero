using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtHours;
    [SerializeField] private TextMeshProUGUI txtGold;
    [SerializeField] private TextMeshProUGUI txtExp;


    public void SetUp(int gold, int exp, int hours)
    {
        txtHours.text = $"Offline Rewards : {hours}h";
        txtGold.text = gold.ToString("N0");
        txtExp.text = exp.ToString("N0");
    }

    public void BtnOK()
    {
        gameObject.SetActive(false);
    }
}
