using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingCtrl : MonoBehaviour
{
    private Volume postProcessing;
    private Vignette vignette;
    private Player player;

    // Vignette Property
    private Coroutine vignetteRoutineInstance;
    private bool isVignetteRoutine = false;
    private bool isIncrease = true;
    private float targetValue = 0.5f;
    private float waitTime = 0.75f; // 비네트 효과 왕복시간


    private void Awake()
    {
        postProcessing = GetComponent<Volume>();

        // Volume.profile.TryGet<T>(out t) => 포스트 프로세싱 볼륨얻기
        postProcessing.profile.TryGet<Vignette>(out vignette);
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer();

        vignette.active = false;
    }

    private void Update()
    {
        // 플레이어 체력이 30% 미만 && 코루틴 한번만 실행
        if (player.Stats.GetHPStatRatio() < 0.3f && !isVignetteRoutine)
        {
            isVignetteRoutine = true;
            vignette.active = true;
            vignetteRoutineInstance = StartCoroutine(vignetteRoutine());
        }

        else if (player.Stats.GetHPStatRatio() >= 0.3f)
        {
            if (vignetteRoutineInstance != null)
            {
                StopCoroutine(vignetteRoutineInstance);
                vignetteRoutineInstance = null;
            }
            vignette.active = false;
            isVignetteRoutine = false;
        }
    }

    private IEnumerator vignetteRoutine()
    {
        // 코루틴이 스탑되었다가 다시 시작될때를 대비해 처음에 초기화해줌
        float elapsedTime = 0.0f; // 경과시간
        float startValue = 0.3f;
        targetValue = 0.5f;
        isIncrease = true; 

        while (true)
        {
            // vignette 크기의 시작값 (0.3 or 0.5)
            startValue = vignette.intensity.value;

            while (elapsedTime < waitTime)
            {
                // vignette의 크기 값을 Lerp로 보간 (시작값, 도착값, 누적시간 / 목표시간)
                vignette.intensity.value = Mathf.Lerp(startValue, targetValue, elapsedTime / waitTime);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // 목표 값을 전환 (0.3~0.5 or 0.5~0.3)
            isIncrease = !isIncrease;
            targetValue = isIncrease ? 0.5f : 0.3f;
            elapsedTime = 0.0f;
        }
    }
}
