using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;
public class AddressableManager : Singleton<AddressableManager>
{
    // 메모리에 올려둔 리소스가 저장된 딕셔너리
    private Dictionary<string, object> resources = new Dictionary<string, object>();
    public IReadOnlyDictionary<string, object> Resources => resources;


    // 리소스를 어드레서블 그룹의 label 단위로 로드
    public async void LoadResources<T>(string label, Action OnComplete = null) where T : Object
    {
        Debug.Log("LoadResources start!");

        try
        {
            var locations = await Addressables.LoadResourceLocationsAsync(label, typeof(T)).Task;
            if (locations == null || locations.Count == 0)
            {
                Debug.LogError($"Failed to load resource locations for label: {label}");
                OnComplete?.Invoke();
                return;
            }

            int totalCount = locations.Count;
            int loadedCount = 0;

            foreach (var resource in locations)
            {
                var obj = await Addressables.LoadAssetAsync<T>(resource).Task;
                if (obj != null)
                {
                    resources[resource.PrimaryKey] = obj;
                }
                else
                {
                    Debug.LogError($"Failed to load asset: {resource.PrimaryKey}");
                }

                loadedCount++;
                // OnProgress?.Invoke((float)loadedCount / totalCount);

                if (loadedCount == totalCount)
                {
                    Debug.Log("Load Complete!" + loadedCount);
                    OnComplete?.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception during resource loading: {ex.Message}");
            OnComplete?.Invoke();
        }
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
