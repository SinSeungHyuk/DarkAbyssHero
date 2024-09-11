using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Image imgLoadingBar;

    private static string nextSceneName;
    private static List<string> resourceLabelsToLoad;

    private float resourceProgress;
    private bool isLoadAll;


    // static 정적함수 : 인스턴스화하지 않고도 아무데서나 호출가능한 로딩함수
    public static void LoadScene(string sceneName, List<string> labelsToLoad)
    {
        nextSceneName = sceneName;
        resourceLabelsToLoad = labelsToLoad;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        // 로딩씬에 진입하면 로딩코루틴 시작
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        imgLoadingBar.fillAmount = 0;

        // 리소스 로딩
        for (int i = 0; i < resourceLabelsToLoad.Count; i++)
        {
            // AddressableManager의 LoadResources 함수를 코루틴으로 호출하여 비동기적으로 진행하기
            yield return StartCoroutine(AddressableManager.Instance.LoadResources<Object>(
                resourceLabelsToLoad[i],
                (progress) =>
                {
                    resourceProgress = (i + progress) / resourceLabelsToLoad.Count;
                    UpdateLoadingProgress(resourceProgress * 0.5f); // 로딩바 절반까지만 채우기
                },
                () => isLoadAll = true
            ));
        }

        // 씬 로딩 비동기 메소드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float sceneProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            UpdateLoadingProgress(0.5f + (sceneProgress * 0.5f)); // 로딩바 나머지 절반 채우기

            // 씬 로딩이 90% 이상이면 allowSceneActivation를 true로 변경하여 씬 변경하기
            if (isLoadAll && asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    private void UpdateLoadingProgress(float progress)
    {
        imgLoadingBar.fillAmount = progress;
    }
}
