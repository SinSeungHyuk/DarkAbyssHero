using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpUI : MonoBehaviour
{

    public void BtnURL()
    {
        // URL : 개발블로그 
        Application.OpenURL(Settings.URL);
    }

    public void BtnOK()
    {
        gameObject.SetActive(false);
    }
}
