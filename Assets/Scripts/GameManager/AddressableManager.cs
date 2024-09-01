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
