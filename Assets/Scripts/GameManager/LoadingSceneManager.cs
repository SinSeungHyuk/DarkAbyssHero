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


    public static void LoadScene(string sceneName, List<string> labelsToLoad)
    {
        nextSceneName = sceneName;
        resourceLabelsToLoad = labelsToLoad;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        imgLoadingBar.fillAmount = 0;

        // 리소스 로딩
        for (int i = 0; i < resourceLabelsToLoad.Count; i++)
        {
            yield return StartCoroutine(AddressableManager.Instance.LoadResources<Object>(
                resourceLabelsToLoad[i],
                (progress) =>
                {
                    resourceProgress = (i + progress) / resourceLabelsToLoad.Count;
                    UpdateLoadingProgress(resourceProgress * 0.5f);  // Resources take up 50% of the loading bar
                },
                () => isLoadAll = true
            ));
        }

        // 씬 로딩
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float sceneProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            UpdateLoadingProgress(0.5f + (sceneProgress * 0.5f));  // Scene loading takes up the other 50%

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
