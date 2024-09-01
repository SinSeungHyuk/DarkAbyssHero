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


    private void Start()
    {
        LoadResources<Database>("Database");
    }

    // 리소스를 어드레서블 그룹의 label 단위로 로드
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
                            // 로드한 리소스 딕셔너리에 추가
                            resources[resource.PrimaryKey] = obj.Result;

                            Debug.Log("LoadResources Database!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        }
                    };
                }
            }
        };
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
