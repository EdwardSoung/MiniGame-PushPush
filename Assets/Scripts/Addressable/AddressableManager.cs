using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using XUnityLibrary.Singleton;

public class AddressableManager : UnitySingleton<AddressableManager>
{
    [SerializeField] private Transform _objectPoolRoot;
    private bool _collectionCheck = false;
    private int _maxPoolSize = 100;
    private Dictionary<string, ObjectPool<GameObject>> _objectPoolDict = new();

    //로드한 에셋 보관
    private Dictionary<string, AsyncOperationHandle> _assetHandleDict = new();

    protected override void AwakeSingleton()
    {
        base.AwakeSingleton();
        Addressables.InitializeAsync();
    }
    public T LoadAsset<T>(string name) where T : UnityEngine.Object
    {
        if (_assetHandleDict.TryGetValue(name, out var cached))
        {
            return cached.Result as T;
        }

        var handle = Addressables.LoadAssetAsync<T>(name);
        handle.WaitForCompletion();

        _assetHandleDict.Add(name, handle);
        return handle.Result;
    }

    public async UniTask<T> LoadAssetAsync<T>(string name) where T : UnityEngine.Object
    {
        if (_assetHandleDict.TryGetValue(name, out var cached))
        {
            return cached.Result as T;
        }

        var handle = Addressables.LoadAssetAsync<T>(name);
        await handle;                        // 로딩 완료까지 대기

        _assetHandleDict.Add(name, handle);
        return handle.Result;
    }

    public void ReleaseAsset(string name)
    {
        if (_assetHandleDict.TryGetValue(name, out var handle))
        {
            Addressables.Release(handle);
            _assetHandleDict.Remove(name);
        }
    }

    //로드된 에셋 전체 해제(씬 전환되거나 할때 용도)
    public void ReleaseAllAssets()
    {
        foreach (var handle in _assetHandleDict.Values)
        {
            Addressables.Release(handle);
        }
        _assetHandleDict.Clear();
    }

    #region ObjectPool
    public GameObject Spawn(string name, Transform parent = null)
    {
        if(!_objectPoolDict.ContainsKey(name))
        {
            var loaded = LoadAsset<GameObject>(name);
            _objectPoolDict.Add(name, CreateNewObjectPool(loaded, name));
        }

        GameObject obj;
        obj = _objectPoolDict[name].Get();
        if(parent == null)
        {
            parent = _objectPoolRoot;
        }
        obj.transform.SetParent(parent);
        return obj;
    }

    public void Release(GameObject obj)
    {
        if (_objectPoolDict.ContainsKey(obj.name))
        {
            try
            {
                obj.transform.parent = null;

                try
                {
                    _objectPoolDict[obj.name].Release(obj);
                }
                catch
                {
                    obj.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception has occured!!! Object : {obj.name} === {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Object[{obj.name}] does not exist in pool dictionary. But it's been destroyed anyway :D");
            Destroy(obj);
        }
    }

    public void ReleaseAll()
    {
        foreach (var pool in _objectPoolDict)
        {
            pool.Value.Dispose();
        }
        _objectPoolDict.Clear();

        for (int i = 0; i < _objectPoolRoot.childCount; i++)
        {
            Destroy(_objectPoolRoot.GetChild(i).gameObject);
        }
    }
    private ObjectPool<GameObject> CreateNewObjectPool(GameObject prefab, string resourceName)
    {
        return new ObjectPool<GameObject>(
                    () =>
                    {
                        GameObject obj = Instantiate(prefab);
                        obj.name = resourceName;
                        return obj;
                    },
                    OnGetFromPoolActive,
                    OnReleasedToPool,
                    OnDestroyPoolObject,
                    _collectionCheck /* Collection checks will throw errors if we try to release an item that is already in the pool.*/,
                    10/*defalut capacity*/,
                    _maxPoolSize
        );
    }

    private void OnGetFromPoolInActive(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnGetFromPoolActive(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReleasedToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject obj)
    {
        Destroy(obj);
    }

    #endregion ObjectPool
}
