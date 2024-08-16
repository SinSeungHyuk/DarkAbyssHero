using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBillboard : MonoBehaviour
{
    private Camera mainCam;

    private void Awake()
    {        
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up);
    }
}
