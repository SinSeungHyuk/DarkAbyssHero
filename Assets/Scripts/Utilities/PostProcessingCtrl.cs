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
    private float waitTime = 0.75f; // ���Ʈ ȿ�� �պ��ð�


    private void Awake()
    {
        postProcessing = GetComponent<Volume>();

        // Volume.profile.TryGet<T>(out t) => ����Ʈ ���μ��� �������
        postProcessing.profile.TryGet<Vignette>(out vignette);
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayer();

        vignette.active = false;
    }

    private void Update()
    {
        // �÷��̾� ü���� 30% �̸� && �ڷ�ƾ �ѹ��� ����
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
        // �ڷ�ƾ�� ��ž�Ǿ��ٰ� �ٽ� ���۵ɶ��� ����� ó���� �ʱ�ȭ����
        float elapsedTime = 0.0f; // ����ð�
        float startValue = 0.3f;
        targetValue = 0.5f;
        isIncrease = true; 

        while (true)
        {
            // vignette ũ���� ���۰� (0.3 or 0.5)
            startValue = vignette.intensity.value;

            while (elapsedTime < waitTime)
            {
                // vignette�� ũ�� ���� Lerp�� ���� (���۰�, ������, �����ð� / ��ǥ�ð�)
                vignette.intensity.value = Mathf.Lerp(startValue, targetValue, elapsedTime / waitTime);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // ��ǥ ���� ��ȯ (0.3~0.5 or 0.5~0.3)
            isIncrease = !isIncrease;
            targetValue = isIncrease ? 0.5f : 0.3f;
            elapsedTime = 0.0f;
        }
    }
}
