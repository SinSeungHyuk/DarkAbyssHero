using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Image imgLoadingBar;

    private static string nextSceneName;
    private static List<string> resourceLabelsToLoad;

    private float resourceProgress;
    private bool isLoadAll;


    // static �����Լ� : �ν��Ͻ�ȭ���� �ʰ� �ƹ������� ȣ�Ⱑ���� �ε��Լ�
    public static void LoadScene(string sceneName, List<string> labelsToLoad)
    {
        nextSceneName = sceneName;
        resourceLabelsToLoad = labelsToLoad;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        // �ε����� �����ϸ� �ε��ڷ�ƾ ����
        LoadSceneAsync().Forget();
    }

    private async UniTask LoadSceneAsync()
    {
        imgLoadingBar.fillAmount = 0;

        // ���ҽ� �ε�
        for (int i = 0; i < resourceLabelsToLoad.Count; i++)
        {
            // AddressableManager�� LoadResources �Լ��� UniTask�� ȣ��
            await AddressableManager.Instance.LoadResources(
                resourceLabelsToLoad[i],
                (progress) =>
                {
                    resourceProgress = (i + progress) / resourceLabelsToLoad.Count;
                    UpdateLoadingProgress(resourceProgress * 0.5f); // �ε��� ���ݱ����� ä���
                },
                () => isLoadAll = true
            );
        }

        // �� �ε� �񵿱� �޼ҵ�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        // �ε��� �Ϸ�� ������ ���
        while (!asyncLoad.isDone)
        {
            float sceneProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            UpdateLoadingProgress(0.5f + (sceneProgress * 0.5f)); // �ε��� ������ ���� ä���

            // �� �ε��� 90% �̻��̸� allowSceneActivation�� true�� �����Ͽ� �� �����ϱ�
            if (isLoadAll && asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            // ������ ��� (UniTask.DelayFrame()�� �̿��Ͽ� �� ������ ���)
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }


    private void UpdateLoadingProgress(float progress)
    {
        imgLoadingBar.fillAmount = progress;
    }
}
