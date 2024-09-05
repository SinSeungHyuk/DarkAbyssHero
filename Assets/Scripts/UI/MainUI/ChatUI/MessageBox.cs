using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtMessage;


    public void SetMessage(string username, string msg)
    {
        txtMessage.text = $"[{username}] : {msg}";
    }
}
