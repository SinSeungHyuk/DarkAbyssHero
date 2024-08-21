using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtNickname;
    [SerializeField] private TextMeshProUGUI txtLevel;

    private Player player;


    void Start()
    {
        player = GameManager.Instance.GetPlayer();

        txtNickname.text = PlayerPrefs.GetString("Nickname");
        txtLevel.text = $"Level : {player.LevelSystem.Level.ToString()}";

        player.LevelSystem.OnLevelChanged += OnLevelTextChanged;
    }

    private void OnLevelTextChanged(LevelSystem system, int level)
    {
        txtLevel.text = $"Level : {level.ToString()}";
    }
}
