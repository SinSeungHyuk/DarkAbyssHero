using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;
public class AddressableManager : Singleton<AddressableManager>
{
    // �޸𸮿� �÷��� ���ҽ��� ����� ��ųʸ�
    private Dictionary<string, object> resources = new Dictionary<string, object>();
    public IReadOnlyDictionary<string, object> Resources => resources;


    // ���ҽ��� ��巹���� �׷��� label ������ �ε�
    public IEnumerator LoadResources<T>(string label, Action<float> progressCallback, Action completionCallback) where T : UnityEngine.Object
    {
        var loadLocationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        yield return loadLocationsHandle;

        if (loadLocationsHandle.Status == AsyncOperationStatus.Succeeded)
        {
            int totalCount = loadLocationsHandle.Result.Count;
            int loadedCount = 0;

            foreach (var resource in loadLocationsHandle.Result)
            {
                var loadAssetHandle = Addressables.LoadAssetAsync<T>(resource);
                yield return loadAssetHandle;

                if (loadAssetHandle.Status == AsyncOperationStatus.Succeeded)
                {
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

        Addressables.Release(loadLocationsHandle);
        completionCallback?.Invoke();
    }


    // ��巹���� �׷쿡 ������ ���ҽ��� key������ ��������
    // key : ��巹���� ������ ���ҽ��� �̸�
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
