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

    private void LateUpdate() // Update에서 카메라가 렌더링하는 화면을 모두 그린 다음에 LateUpdate에서 각도조정
    {
        // UI가 카메라를 향해 바라보게 하기 
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up);
    }
}
