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


    private void Start()
    {
        LoadResources<Database>("Database");
    }

    // ���ҽ��� ��巹���� �׷��� label ������ �ε�
    public void LoadResources<T>(string label) where T : Object
    {
        Addressables.LoadResourceLocationsAsync(label, typeof(T)).Completed += (group) =>
        {
            if (group.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var resource in group.Result)
                {
                    Addressables.LoadAssetAsync<T>(resource).Completed += (obj) =>
                    {
                        if (obj.Status == AsyncOperationStatus.Succeeded)
                        {
                            // �ε��� ���ҽ� ��ųʸ��� �߰�
                            resources[resource.PrimaryKey] = obj.Result;

                            Debug.Log("LoadResources Database!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }
                    };
                }
            }
        };
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
