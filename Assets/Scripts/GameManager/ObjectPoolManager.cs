using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    // �ν����Ϳ��� ����� Pool
    [SerializeField] private List<Pool> poolArray = new List<Pool>();
    private Dictionary<string, ObjectPool> poolDic = new Dictionary<string, ObjectPool>();
    private Transform objPoolTransform;

    [System.Serializable]
    public struct Pool
    {
        public string name;
        public int initialSize;
        public GameObject prefab;
    }

    private class ObjectPool
    {
        // ���� ���� Ȱ��ȭ �Ǿ��ִ� ������Ʈ�� ����Ʈ
        public List<GameObject> activeObjects = new List<GameObject>();
        // ��Ȱ��ȭ �Ǿ��ִ� ������� ������Ʈ�� ť
        public Queue<GameObject> inactiveObjects = new Queue<GameObject>();
    }

    private void Start()
    {
        objPoolTransform = this.gameObject.transform;
        for (int i = 0; i < poolArray.Count; ++i)        
            CreatePool(poolArray[i].prefab, poolArray[i].initialSize, poolArray[i].name);        
    }

    private void CreatePool(GameObject prefab, int size, string name)
    {
        GameObject poolContainer = new GameObject(name);
        poolContainer.transform.SetParent(objPoolTransform);

        ObjectPool objectPool = new ObjectPool();
        for (int i = 0; i < size; ++i)
        {
            GameObject obj = CreateNewObject(prefab, poolContainer.transform);
            // ó�� ������ ������Ʈ���� ���ť�� ���� ���
            objectPool.inactiveObjects.Enqueue(obj);
        }
        poolDic.Add(name, objectPool);
    }

    private GameObject CreateNewObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get(string name, Vector3 position, Quaternion rotation)
    {
        if (poolDic.TryGetValue(name, out ObjectPool objectPool))
        {
            GameObject obj;
            // ������� ������Ʈ�� �ִ� ť�� ����
            if (objectPool.inactiveObjects.Count == 0)
            {
                // Ǯ�� ��� ������Ʈ�� Ȱ��ȭ�Ǿ��� �� Ǯ Ȯ��
                GameObject prefab = poolArray.Find(p => p.name == name).prefab;
                obj = CreateNewObject(prefab, objPoolTransform.Find(name));
            }
            else            
                // ������� ť���� ������Ʈ �ϳ� ��������
                obj = objectPool.inactiveObjects.Dequeue();
            

            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            // Ȱ��ȭ �� ������Ʈ�� Ȱ��ȭ ����Ʈ�� ����
            objectPool.activeObjects.Add(obj);
            return obj;
        }

        return null;
    }
    public GameObject Get(string name, Transform transform)
        => Get(name, transform.position, transform.rotation);

    public void Release(GameObject obj, string poolName)
    {
        if (poolDic.TryGetValue(poolName, out ObjectPool objectPool))
        {
            // ��ȯ�� ������Ʈ�� Ȱ��ȭ ����Ʈ���� ����
            if (objectPool.activeObjects.Remove(obj))
            {
                // ��ȯ�� ������Ʈ�� ��� ť�� �־��ֱ�
                obj.SetActive(false);
                objectPool.inactiveObjects.Enqueue(obj);
            }
        }
    }
}