using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Player player;


    

    public Player GetPlayer() => player;
}
