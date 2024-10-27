using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Threading;

using Object = UnityEngine.Object;
public class AddressableManager : Singleton<AddressableManager>
{
    // 메모리에 올려둔 리소스가 저장된 딕셔너리
    private Dictionary<string, object> resources = new Dictionary<string, object>();
    public IReadOnlyDictionary<string, object> Resources => resources;


    // 리소스를 어드레서블 그룹의 label 단위로 로드
    public async UniTask LoadResources(string label, Action<float> progressCallback, Action completionCallback) 
    {
        // 1. LoadResourceLocationsAsync : 매개변수로 받은 Label 단위로 해당 경로의 리소스 로드
        var loadLocationsHandle = Addressables.LoadResourceLocationsAsync(label);
        await loadLocationsHandle; // 모두 로드될때까지 대기 (비동기 메소드지만 마치 동기메소드처럼 동작)

        if (loadLocationsHandle.Status == AsyncOperationStatus.Succeeded)
        {
            int totalCount = loadLocationsHandle.Result.Count;
            int loadedCount = 0;

            // 위에서 로드한 해당 Label 경로에 들어있는 모든 리소스들 하나하나 순회하며 로드
            foreach (var resource in loadLocationsHandle.Result)
            {
                var loadAssetHandle = Addressables.LoadAssetAsync<Object>(resource); // LoadAssetAsync<T> : 리소스 로드하는 기본함수
                await loadAssetHandle;

                if (loadAssetHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    // 리소스를 하나 로드할때마다 진행도 높여서 콜백함수 호출
                    resources[resource.PrimaryKey] = loadAssetHandle.Result;
                    loadedCount++;
                    progressCallback?.Invoke((float)loadedCount / totalCount);
                }
                else
                {
                    Debug.LogError($"Failed to load resource: {resource.PrimaryKey}");
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load resource locations for label: {label}");
        }

        Addressables.Release(loadLocationsHandle); // 각 리소스 모두 로드되었으므로 필요없는 리소스는 해지
        completionCallback?.Invoke(); // 로드 완료
    }


    // 어드레서블 그룹에 저장한 리소스의 key값으로 가져오기
    // key : 어드레서블에 저장한 리소스의 이름
    public T GetResource<T>(string key) where T : Object
    {
        if (Resources.TryGetValue(key, out var resource))
        {
            return resource as T;
        }
        return null;
    }

    public void ReleaseAll()
    {
        if (resources.Count == 0) return;

        foreach (var resource in resources)
        {
            Addressables.Release(resource.Value);
        }
    }
    private void OnDestroy()
    {
        ReleaseAll();
    }
}
